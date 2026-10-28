# Invoice Approval Service (Tier-2 Production Ready)

A .NET 8 invoice approval service designed as a Tier-2 service with a 24x7 on-call rotation, a published SLO, and an outbound integration with the PayEx payment gateway. This repository contains the full application codebase, architecture governance tests, operational risk audits, an AI review evaluation suite, and human-in-the-loop AI-in-SDLC audit logs[cite: 3, 4].

---

## Demo Video

- **File Location**: `demo/demo-video.mp4` (or within the `demo/` folder)[cite: 3, 4]
- **Duration**: ~8 to 10 minutes
- **Contents Covered**:
  - As-built and target architecture diagrams with symbol citations and inferred items[cite: 3, 4].
  - Execution of the architecture boundary unit test (`dotnet test`)[cite: 3, 4].
  - Exactly-once PayEx integration ADR defense, including 3 a.m. peak stress test and agent limitations[cite: 3, 4].
  - First-response incident triage autonomy policy (Day-One Level 1 selection)[cite: 3, 4].
  - Review evaluation suite (12 cases across 4 runs) and statistical findings[cite: 4].
  - Lethal trifecta risk audit and baseline measurement plan[cite: 3, 4].
  - AI augmentation audit logs review[cite: 4].

---

## Deliverables & Documentation Index

| Deliverable | Location | Description |
|---|---|---|
| **Architecture Diagrams & Citations**[cite: 3, 4] | `docs/architecture/as-built-and-target-diagrams.md` | Container & Sequence diagrams (C4/Mermaid) with file/symbol citations and inferred items list[cite: 3, 4]. |
| **Architecture Unit Test**[cite: 3, 4] | `backend/tests/InvoiceApp.Tests/ArchitectureTests.cs` | Reflection-based test enforcing that Domain has zero external dependencies[cite: 3, 4]. |
| **Exactly-Once Submission ADR**[cite: 3, 4] | `docs/adr/ADR-001-exactly-once-payex-submission.md` | Transactional Outbox ADR, 3 a.m. peak stress-test, and *"Where the agent was wrong or useless"* section[cite: 3, 4]. |
| **Autonomy Policy**[cite: 3, 4] | `docs/policies/autonomy-policy-incident-response.md` | One-page policy for first-response incident log triage (Day-One Level: L1 Assistive)[cite: 3, 4]. |
| **Review Evaluation Suite**[cite: 4] | `ops/evals/` | 12 diff cases (`case-01` to `case-12`), `results-template.csv`, and `EVALUATION_REPORT.md`[cite: 1, 4]. |
| **Lethal Trifecta Risk Audit**[cite: 3, 4] | `ops/audits/lethal-trifecta-risk-audit.md` | Security audit eliminating automated execution loops in AI triage agents[cite: 3, 4]. |
| **Baseline Measurement Plan**[cite: 3, 4] | `ops/metrics/baseline-measurement-plan.md` | DORA metrics plan + 2 measured baselines (11.85 min CI time, 27.4 hr PR turnaround)[cite: 3, 4]. |
| **AI Augmentation Logs**[cite: 4] | `audit-logs/` | 12+ chronological entries (`.jsonl`, `.json`, and `AI_AUGMENTATION_SUMMARY.md`)[cite: 4]. |

---

## Quickstart & Local Execution

### 1. Run the Web Application

```bash
dotnet run --project backend/src/InvoiceApp.Api