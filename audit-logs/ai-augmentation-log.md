# AI augmentation log

Tool: an AI assistant in VS Code (Copilot SDK). The session record shows two models: `claude-sonnet-5.5` (auto-selected for the main session) and `gpt-5.6-luna` (the `explore` sub-agent default, reasoning effort low, which ran evaluation runs 1-4; `claude-sonnet-5` was set explicitly for evaluation run 5). Session window 2026-10-30 04:50-06:16 UTC; 71 user messages; 261 tool calls (50 sub-agent launches, 57 sub-agent reads, 46 shell, 73 file views, 21 create/edit, 3 draw.io diagrams).

A content-free timeline of every tool call (UTC time and tool name only, no prompts, outputs or paths) is in [session-tool-timeline.csv](session-tool-timeline.csv). The full raw record is not included because it holds paths and tool output.

This file replaces an earlier `audit-logs/` set (a JSON, a JSONL and a summary table) that named a different model, listed timestamps and a "few-shot prompt" fix, and cited an 11.85-minute CI baseline. None of those could be traced to this work, and the 11.85 figure does not reproduce, so they were removed (they remain in git history).

| # | Task | What the AI did | What I verified or corrected | Outcome |
| --- | --- | --- | --- | --- |
| 1 | Architecture diagrams | Read the code, installed the .NET 8 SDK (only the runtime was present), drew container, sequence and target diagrams via the draw.io Mermaid MCP, wrote citation and "inferred" lists | Corrected wrong claims in the earlier doc, e.g. a sentence saying the ADR names no cloud products | [as-built-and-target-diagrams.md](../docs/architecture/as-built-and-target-diagrams.md) |
| 2 | Architecture test | Wrote `Domain_Project_Declares_No_Dependencies` | The earlier test (`GetReferencedAssemblies`) could pass vacuously because the compiler drops unused references. I confirmed the new test fails when Newtonsoft.Json is added to the Domain project, then restored it. Full suite passes 10/10 | [ArchitectureTests.cs](../backend/tests/InvoiceApp.Tests/ArchitectureTests.cs) |
| 3 | Exactly-once ADR | Edited the existing record | Removed "broker outages" wording and a $140/month estimate that had no basis in the decision; added items 5-7 to "Where the agent was wrong or useless". Removed four earlier items (two-phase commit, retry on timeout, broker dedup, overstated guarantees) because the session record never shows the agent saying them | [ADR-001](../docs/adr/ADR-001-exactly-once-payex-submission.md) |
| 4 | Autonomy policy | Rewrote to about one page | Day-one level L1 with conditions for the two new engineers | [policy](../docs/policies/autonomy-policy-incident-response.md) |
| 5 | Review evaluation | Ran 5 passes x 12 cases with isolated reviewer agents (runs 1-4 on gpt-5.6-luna, run 5 on claude-sonnet-5), scored them | Replaced the earlier GPT-4o figures, which I could not trace to any run. I scored the runs myself, which is a bias. The one prompt change was aimed at my own case 11 | [report](../ops/evals/EVALUATION_REPORT.md), [results.csv](../ops/evals/results.csv) |
| 6 | Risk audit | Audited the agent setup against the lethal trifecta | `ops/README.md` has no agent setup section; I audited the setup in use here and said so. Withdrew an earlier audit of a reviewer that no file describes | [audit](../ops/audits/lethal-trifecta-risk-audit.md) |
| 7 | Measurement plan | Computed CI duration and failure rate from `run-timings.csv` | Removed earlier figures (11.85 min, 27.4 h) that do not reproduce | [plan](../ops/metrics/baseline-measurement-plan.md) |

## Where the AI was wrong or needed correcting in this work

- Wrote a plausible but untraceable evaluation report and baseline numbers; both were replaced.
- Audited a scenario (a merging PR bot) that did not exist in the repo.
- Wrote the changed evaluation prompt after knowing case 11, so its single gain is the defect it targeted; the report says this.
- A delete command raced with file creation, so two writes failed; the files were recreated.

## Limits

- This table is a summary written from the session. The timeline CSV is machine-generated from the raw record; the "verified or corrected" column is the AI's own account and the author should confirm it.
- The demo video is recorded separately; its script is [demo-video-outline.md](../docs/demo-video-outline.md).




