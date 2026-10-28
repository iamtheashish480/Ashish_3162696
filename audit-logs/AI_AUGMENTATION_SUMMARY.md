# AI Augmentation Summary Table

| Timestamp | Stage | Agent / Model | Prompt / Request | Limitation / Failure Identified | Human Decision / Fix | Status |
|---|---|---|---|---|---|:---:|
| 2026-10-28 10:15 | setup | Claude 3.7 Sonnet | Repo analysis & test suite check | None | Configured test project & xUnit runner | `applied` |
| 2026-10-28 10:45 | plan | Claude 3.7 Sonnet | As-built & target Mermaid diagrams | Hallucinated EF Core DB in as-built state | Rewrote to reflect `InMemoryInvoiceRepository` | `rework_required` |
| 2026-10-28 11:30 | implementation | Claude 3.7 Sonnet | Architecture unit test | Only checked namespaces, missed assemblies | Added reflection-based reference barrier & entity setter check | `partially_applied` |
| 2026-10-28 12:15 | implementation | Claude 3.7 Sonnet | Exactly-once PayEx ADR | Proposed naive HTTP dual-write | Enforced Transactional Outbox pattern | `rework_required` |
| 2026-10-28 13:00 | implementation | Claude 3.7 Sonnet | Incident autonomy policy | Recommended L2 posting without onboarding check | Set Day-One level to L1 (Assistive) | `partially_applied` |
| 2026-10-28 14:00 | testing | Claude 3.7 Sonnet | 12 evaluation test passes | Missed race condition in baseline run | Added few-shot prompt to catch concurrency flaws | `applied` |
| 2026-10-28 15:00 | verification | Claude 3.7 Sonnet | Lethal trifecta audit & DORA baselines | None | Formulated human gatekeeper policy and verified 11.85 min CI baseline | `applied` |