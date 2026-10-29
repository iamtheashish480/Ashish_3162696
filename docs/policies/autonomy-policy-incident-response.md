# Autonomy Policy: First-Response Incident Summaries

## Scope and Objective
Defines the safe operational parameters for using AI agents to synthesize application logs, telemetry, and metrics into first-response incident briefs during on-call rotations.

## Autonomy Matrix
| Level | Agent Authority | Guardrails & Verification | Human Approval Gate |
|---|---|---|---|
| **L1: Assistive** | Read-only extraction: summarizes logs and metrics upon explicit on-call engineer CLI invocation. | Output tagged as `#draft-summary`; runs in sandboxed read-only Azure Monitor scope. | On-call engineer reads and edits draft before posting to incident channel. |
| **L2: Semi-Autonomous** | Automatically triggers on P1/P2 alerts; parses stack traces and posts summary directly to incident Slack thread. | Restricted from running remediation commands or closing alerts; strictly observational. | Incident Commander signs off on summary accuracy during handoff. |
| **L3: Conditional Autonomy** | Correlates logs, suggests root cause hypotheses, and drafts remediation scripts (e.g., scale app service, restart worker). | Remediation scripts cannot execute automatically; syntax and scope validated via dry-run. | Explicit Senior Engineer / Incident Commander approval required to run actions. |
| **L4: Autonomous** | Executes non-destructive runbooks (e.g., rolling restart, traffic diversion). | Hard limits on blast radius; automatic rollback if error rate spikes > 1%. | Full automated runbook post-mortem reviewed within 24h by Tech Lead. |

## Promotion Criteria: L1 to L2
1. **Evaluation Baseline:** Agent must complete 30 consecutive incident reviews with zero hallucinations of stack traces or metric bounds.
2. **False Positive Rate:** < 5% irrelevant root cause suggestions.
3. **Sign-off:** Approved in writing by the Team Lead and Staff Site Reliability Engineer.

## Day-One Operational Decision
**Designation: Level 1 (Assistive Only)**

**Justification:** With 2 out of 6 engineers completely new to the codebase, the team lacks the institutional memory needed to instantly spot subtle AI misinterpretations in log triage. Starting at L1 ensures that every automated summary is vetted by a human engineer, preventing premature or faulty incident mitigation.