# Architecture Diagrams & System Evolution

## 1. As-Built Container View (C4 Level 2)

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
        DiscountCalc["DiscountCalculator"]
        InvoiceModel["Invoice Entity & InvoiceStatus"]
    end

    User -->|HTTP GET /| StaticFiles
    User -->|HTTP POST /api/auth| AuthCtrl
    AuthCtrl --> SessionStore
    User -->|HTTP REST /api/invoices, /api/approvals| SessionAuth
    SessionAuth --> ApprovalsCtrl
    SessionAuth --> InvoicesCtrl
    ApprovalsCtrl --> ApprovalSvc
    InvoicesCtrl --> Repo
    ApprovalSvc --> Repo
    ApprovalSvc --> DiscountCalc
    ApprovalSvc --> InvoiceModel
```

### Component Citations
* `Static UI Files`: `backend/src/InvoiceApp.Api/wwwroot/index.html`, `app.js`, `styles.css`
* `SessionAuthMiddleware`: `backend/src/InvoiceApp.Api/Auth/SessionAuthMiddleware.cs`
* `SessionStore`: `backend/src/InvoiceApp.Api/Auth/SessionStore.cs`
* `ApprovalsController`: `backend/src/InvoiceApp.Api/Controllers/ApprovalsController.cs`
* `InvoicesController`: `backend/src/InvoiceApp.Api/Controllers/InvoicesController.cs`
* `InMemoryInvoiceRepository`: `backend/src/InvoiceApp.Api/InMemoryInvoiceRepository.cs`
* `ApprovalService`: `backend/src/InvoiceApp.Domain/ApprovalService.cs`
* `DiscountCalculator`: `backend/src/InvoiceApp.Domain/DiscountCalculator.cs`
* `Invoice`: `backend/src/InvoiceApp.Domain/Invoice.cs`

### Inferred Items Corrected Against Codebase
| Agent Inferred Item | Actual Codebase Reality |
|---|---|
| Assumed database connection via EF Core | Entirely in-memory (`InMemoryInvoiceRepository` using a `ConcurrentDictionary`) |
| Assumed JWT Bearer tokens | Simple custom header `X-Session-Id` mapped in memory |
| Assumed external notification worker | No background workers; synchronous HTTP request-response only |

---

## 2. As-Built Approval Sequence Diagram

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
    UI->>Ctrl: POST /api/approvals/{id} (Headers: X-Session-Id)
    Ctrl->>Svc: ApproveAsync(id, approverRole)
    Svc->>Repo: GetByIdAsync(id)
    Repo-->>Svc: Invoice entity
    Svc->>Model: CanTransitionTo(Approved) & SetStatus(Approved)
    Svc->>Repo: SaveAsync(invoice)
    Svc-->>Ctrl: Result<Invoice>
    Ctrl-->>UI: 200 OK (Updated Invoice JSON)
    UI-->>User: Refresh list & status badge
```

---

## 3. Target State Architecture (Tier-2 Production)

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
*(Items highlighted with dashed borders represent planned components not yet present in codebase).*