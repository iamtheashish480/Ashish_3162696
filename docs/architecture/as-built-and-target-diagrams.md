# Architecture Diagrams and System Evolution

## As-Built Container View (C4 Level 2)

```mermaid
graph TD
    User([User Browser])
    subgraph "ASP.NET Core Host (InvoiceApp.Api)"
        StaticFiles["Static UI Files (index.html, app.js, styles.css)"]
        SessionAuth["SessionAuthMiddleware / RequireRole"]
        InvoicesCtrl["InvoicesController"]
        ApprovalsCtrl["ApprovalsController"]
        AuthCtrl["AuthController"]
        SessionStore["SessionStore"]
        Repo["InMemoryInvoiceRepository"]
    end
    subgraph "InvoiceApp.Domain"
        ApprovalSvc["ApprovalService"]
        DiscountCalc["DiscountCalculator (not in these flows)"]
        InvoiceModel["Invoice Entity & InvoiceStatus"]
    end

    User -->|HTTP GET /| StaticFiles
    User -->|POST /api/auth/login| AuthCtrl
    AuthCtrl -->|Create opaque bearer token| SessionStore
    User -->|Bearer token on protected API requests| SessionAuth
    SessionAuth --> ApprovalsCtrl
    SessionAuth --> InvoicesCtrl
    ApprovalsCtrl --> ApprovalSvc
    InvoicesCtrl --> Repo
    InvoicesCtrl --> ApprovalSvc
    ApprovalSvc --> Repo
    ApprovalSvc --> InvoiceModel
```

### Component Citations
- `Static UI files`: `backend/src/InvoiceApp.Api/wwwroot/index.html`, `app.js`, `styles.css`
- `SessionAuthMiddleware`: `backend/src/InvoiceApp.Api/Auth/SessionAuthMiddleware.cs`
- `SessionStore`: `backend/src/InvoiceApp.Api/Auth/SessionStore.cs`
- `AuthController`: `backend/src/InvoiceApp.Api/Controllers/AuthController.cs`
- `ApprovalsController`: `backend/src/InvoiceApp.Api/Controllers/ApprovalsController.cs`
- `InvoicesController`: `backend/src/InvoiceApp.Api/Controllers/InvoicesController.cs`
- `InMemoryInvoiceRepository`: `backend/src/InvoiceApp.Api/InMemoryInvoiceRepository.cs`
- `ApprovalService`: `backend/src/InvoiceApp.Domain/ApprovalService.cs`
- `DiscountCalculator`: `backend/src/InvoiceApp.Domain/DiscountCalculator.cs`
- `Invoice`: `backend/src/InvoiceApp.Domain/Invoice.cs`

### Inferences Corrected Against the Codebase

| Agent-inferred item | Actual codebase reality |
| --- | --- |
| Assumed database connection via EF Core | Entirely in memory; `InMemoryInvoiceRepository` stores invoices in a lock-protected `List<Invoice>`. |
| Assumed JWT Bearer tokens | Simple custom header `X-Session-Id` mapped in memory |
| Assumed external notification worker | No background workers; requests are handled synchronously. |

---

## As-Built Approval Sequence Diagram

```mermaid
sequenceDiagram
    autonumber
    actor User as Approver
    participant UI as wwwroot/app.js
    participant Ctrl as ApprovalsController
    participant Svc as ApprovalService
    participant Repo as InMemoryInvoiceRepository
    participant Model as Invoice

    User->>UI: Clicks "Approve"
    UI->>Ctrl: POST /api/approvals/{id}/approve (Authorization: Bearer token)
    Ctrl->>Svc: Approve(id, session.Username)
    Svc->>Repo: GetById(id)
    Repo-->>Svc: Invoice entity
    Svc->>Model: Approve(approver)
    Svc->>Repo: Update(invoice)
    Svc-->>Ctrl: Invoice
    Ctrl-->>UI: 200 OK (Updated Invoice JSON)
    UI-->>User: Refresh list & status badge
```

---

## Target State Architecture (Tier-2 Production)

```mermaid
graph TD
    Client([Approver Web Client])
    
    subgraph AppService["Azure App Service (Tier-2)"]
        API["InvoiceApp.Api (Stateless)"]
        OutboxProcessor["Background Outbox Worker (IHostedService)"]
    end

    subgraph AzureData["Data Tier"]
        SQL[(Azure SQL Database)]
        subgraph Tables["Relational Tables"]
            InvTable[Invoices]
            AuditTable[ApprovalAuditEvents]
            OutboxTable[PaymentOutbox]
            DedupeTable[IdempotencyKeys]
        end
    end

    subgraph Messaging["Async Integration"]
        ASB[Azure Service Bus Topic: invoice-approved]
    end

    subgraph External["Third Party"]
        PayEx[PayEx Payment Gateway API]
    end

    Client -->|HTTPS| API
    API -->|ACID Transaction| SQL
    SQL --- InvTable
    SQL --- OutboxTable
    SQL --- AuditTable
    SQL --- DedupeTable
    
    OutboxProcessor -->|Poll / CDC| OutboxTable
    OutboxProcessor -->|Publish| ASB
    ASB -->|Compensating Retry Loop| PayEx
    
    classDef planned fill:#f9f,stroke:#333,stroke-dasharray: 5 5;
    class SQL,OutboxProcessor,ASB,PayEx planned;
```
> Items highlighted with dashed borders represent planned components that are not yet present in the codebase.