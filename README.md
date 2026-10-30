# Invoice Approval Service: Tier-2 Production Readiness

An invoice approval demo app (ASP.NET Core 8 Web API with a small static web UI), plus the engineering artefacts for taking it to tier-2 production: architecture diagrams and a test, a decision record, an autonomy policy, a review evaluation, a risk audit and a measurement plan.

> **What is real and what is planned.** The code runs on an in-memory store. The database, message topic and PayEx payment integration described under `ops/` and in the target-state diagram are a **planned** deployment and do not exist in the code. Nothing under `ops/` describes what the service does today.

## Contents

- [Key features](#key-features)
- [Quick start](#quick-start)
- [Repository structure](#repository-structure)
- [Deliverables](#deliverables)
- [Design summary](#design-summary)
- [Headline results](#headline-results)
- [Scope notes](#scope-notes)
- [Pre-submission checklist](#pre-submission-checklist)

## Key features

- **Approval workflow:** invoices move through Draft, PendingApproval, Approved and Rejected, owned by `ApprovalService` in the Domain project. Sign-in with Approver and Admin roles.
- **Architecture governance:** executable tests for the domain's dependency boundary and entity state encapsulation.
- **Architecture documentation:** as-built and target-state Mermaid diagrams with a citation for every element and a separate list of inferred items.
- **Payment reliability (design only):** an ADR for exactly-once submission to PayEx.
- **Operational readiness:** an incident-response autonomy policy and a baseline measurement plan.
- **Security:** a lethal-trifecta risk audit of the agent setup.
- **AI evaluation:** twelve diff cases, five recorded runs on two models.
- **Auditability:** an AI augmentation log with a tool timeline.

## Quick start

Requires the [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) and Git.

```powershell
git clone https://github.com/iamtheashish480/Ashish_3162696.git
cd Ashish_3162696
dotnet restore backend/InvoiceApp.sln
dotnet build backend/InvoiceApp.sln
dotnet test backend/InvoiceApp.sln                 # 10 tests
dotnet run --project backend/src/InvoiceApp.Api    # then open http://localhost:5000
```

Run only the architecture tests:

```powershell
dotnet test backend/tests/InvoiceApp.Tests/InvoiceApp.Tests.csproj --filter FullyQualifiedName~ArchitectureTests
```

End-to-end tests (Playwright):

```powershell
cd e2e; npm install; npx playwright install chromium; npm test
```

Demo accounts (not real credentials), password `ChangeMe123!`:

| User | Role |
| --- | --- |
| `demo.approver` | Approver |
| `demo.admin` | Admin |

## Repository structure

```text
.
├── backend/
│   ├── InvoiceApp.sln
│   ├── src/
│   │   ├── InvoiceApp.Domain/     business rules, no web or storage code
│   │   └── InvoiceApp.Api/        controllers, Auth/, in-memory repository, wwwroot/ UI
│   └── tests/InvoiceApp.Tests/    7 approval-service tests + 3 architecture tests
├── e2e/                           Playwright tests and page objects
├── docs/
│   ├── architecture/as-built-and-target-diagrams.md
│   ├── adr/ADR-001-exactly-once-payex-submission.md
│   ├── policies/autonomy-policy-incident-response.md
│   └── demo-video-outline.md
├── ops/                           describes the PLANNED deployment
│   ├── incidents/                 INC-2041, INC-2047 evidence
│   ├── pipeline/                  ci-pipeline.yml, run-timings.csv
│   ├── evals/                     cases/, results.csv, EVALUATION_REPORT.md
│   ├── audits/lethal-trifecta-risk-audit.md
│   └── metrics/baseline-measurement-plan.md
├── audit-logs/                    ai-augmentation-log.md, session-tool-timeline.csv
└── AGENTS.md                      project rules for AI agents
```

## Deliverables

| # | Deliverable | Where |
| --- | --- | --- |
| 1 | As-built and target diagrams (Mermaid), citations, inferred-items list | [docs/architecture/as-built-and-target-diagrams.md](docs/architecture/as-built-and-target-diagrams.md) |
| 2 | Architecture test `Domain_Project_Declares_No_Dependencies` | [ArchitectureTests.cs](backend/tests/InvoiceApp.Tests/ArchitectureTests.cs) |
| 3 | Exactly-once decision record, with "Where the agent was wrong or useless" | [docs/adr/ADR-001-exactly-once-payex-submission.md](docs/adr/ADR-001-exactly-once-payex-submission.md) |
| 4 | One-page autonomy policy (first-response incident summaries, day one = L1) | [docs/policies/autonomy-policy-incident-response.md](docs/policies/autonomy-policy-incident-response.md) |
| 5 | Review evaluation: two added cases (11, 12), 3 baseline runs, 1 changed-prompt run, 1 second-model run | [ops/evals/EVALUATION_REPORT.md](ops/evals/EVALUATION_REPORT.md), [results.csv](ops/evals/results.csv), [cases/](ops/evals/cases) |
| 6 | Lethal trifecta risk audit, with decision | [ops/audits/lethal-trifecta-risk-audit.md](ops/audits/lethal-trifecta-risk-audit.md) |
| 7 | Measurement plan, with two captured numbers | [ops/metrics/baseline-measurement-plan.md](ops/metrics/baseline-measurement-plan.md) |
| 8 | Demo video (8-10 min) | Uploaded separately; link: `<paste video URL here>`. Script: [docs/demo-video-outline.md](docs/demo-video-outline.md) |
| 9 | AI augmentation log | [audit-logs/ai-augmentation-log.md](audit-logs/ai-augmentation-log.md), [session-tool-timeline.csv](audit-logs/session-tool-timeline.csv) |

## Design summary

**Architecture.** The Domain project declares no project, package or framework references, and a test enforces it. The as-built diagrams show the in-memory repository and the approval path from controller to `ApprovalService`; the target diagram marks every part that does not exist yet as NOT BUILT.

**PayEx submission (decision, not implementation).** The ADR chooses a transactional outbox with a worker that calls PayEx directly, using a stable idempotency key. An outbox alone does not make a payment exactly-once: PayEx can time out after 30 seconds without saying whether it accepted the request, so the worker records an unknown outcome and reconciles it rather than blindly retrying. Capacity and cost claims stay unpriced until PayEx limits and volumes are known. See the ADR for options considered, failure modes and revisit conditions.

**Autonomy.** The policy covers first-response incident summaries. Day one is L1 (assistive, on request), with extra review conditions because two of the six engineers are new to the codebase.

**Risk.** The audit finds all three lethal-trifecta legs present in the coding-agent setup (private data, untrusted content, outward actions) and removes the outward-communication leg. Controls must be real tool permissions, not instructions to the model.

## Headline results

- **Architecture test:** fails if the Domain project declares any project, package or framework reference. Verified by adding a package to the Domain project and watching it fail.
- **Evaluation** (12 cases: 9 defects, 3 clean; 5 runs, 60 rows):
  - Original prompt on `gpt-5.6-luna`, three runs: caught 23/27 defect reviews (85%), missed 4/27, 0 false alarms in 9 clean reviews.
  - Changed prompt (run 4): caught 8/9, but 2 false alarms in 3 clean reviews. Not a clear improvement; confidence is low.
  - Second model `claude-sonnet-5`, original prompt (run 5): caught 8/9, 1 borderline false alarm.
  - Case 11 (an architecture guard that can pass without checking anything) was missed by every original-prompt run on both models.
- **Baseline** from `ops/pipeline/run-timings.csv` (20 CI runs): mean CI duration 31.3 minutes (median 31.3, p90 34.5); CI failure rate 10% (2 of 20), a stand-in for change failure rate.
- **Risk audit:** the decision removes the outward-communication leg.

## Scope notes

- **Baseline:** two numbers were captured from the repository's CI history, as the brief asks. Lead time and change failure rate in the full DORA sense need deployments, which do not exist yet, so the plan records their queries instead. Review turnaround is handled the same way: the repository has no pull requests to measure.
- **Risk audit:** it covers the coding-agent setup, described in the final section of [ops/README.md](ops/README.md).
- **AI augmentation log:** the "verified or corrected" column is the AI's own account; the timeline CSV beside it is generated from the session record.
- **Evaluation:** a small sample (12 cases; one run each of the changed prompt and the second model), so conclusions are stated with low confidence.

## Pre-submission checklist

- [x] The solution restores and builds (0 warnings, 0 errors).
- [x] All 10 automated tests pass.
- [x] Diagrams distinguish what exists from what is planned.
- [x] Evaluation figures match [results.csv](ops/evals/results.csv).
- [x] Baseline numbers reproduce from `ops/pipeline/run-timings.csv`.
- [x] Relative links in this README resolve.
- [ ] Demo video uploaded and its URL pasted into deliverable 8.
