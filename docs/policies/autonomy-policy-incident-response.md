# AI Autonomy Policy: First-Response Incident Summaries

Scope: AI-drafted first-response briefs from logs, telemetry and metrics during on-call. This is the planned production model; the demo app has no monitoring, paging or payment integration. Levels are granted per task by people, never by the agent or by metrics alone.

## 1. Tasks by level

| Task | Level | Status |
| --- | --- | --- |
| Draft a brief from logs and metrics the on-call engineer names | L1 Assistive (on request) | **Allowed day one** |
| Propose 2-3 mitigation options with effect, risk, cost, rollback | L1 | **Allowed day one** (options only, never ranked as approved) |
| List unrecovered business effects (missed payments, notifications) for follow-up | L1 | **Allowed day one** |
| Auto-draft a summary when a P1/P2 alert fires, for the incident channel | L2 Triggered | Not approved |
| Correlate evidence and draft remediation steps | L3 Proposes | Not approved |
| Run a pre-approved runbook | L4 Autonomous | Not approved |
| Restart, roll back, change config or data, close or ack alerts, set severity, page people, post externally | none | **Never for the agent** at any level in this policy |

## 2. Controls that make L1 safe

- **Read-only identity**, enforced by permissions and tool config, not by prompt wording. No production write credentials or unrelated secrets.
- **Labelled draft:** `DRAFT - HUMAN REVIEW REQUIRED`. Facts separated from hypotheses; each fact cites source and timestamp; unknowns stated. No invented log lines or metric values.
- **Human verification:** the on-call engineer checks every material claim against the source before sharing. A named approver authorises any production change; out of hours that is the incident commander.
- **Data minimisation:** only approved sources; if the handling path is unclear, do not submit the evidence.
- **Record:** the reviewed brief, evidence window, model and tool version, human edits and reviewer role go in the approved incident system.

## 3. Evidence to move a task up one level (L1 to L2 shown)

All of the following, recorded with case-level counts:

1. At least 30 consecutive scored incident reviews with zero fabricated stack traces or metric values.
2. Wrong root-cause suggestions below 5% of all root-cause suggestions assessed (counts and calculation recorded).
3. Test cases that include incomplete and contradictory evidence.
4. A written review of access, data handling, failure behaviour and the approval workflow for the proposed L2 scope.

Meeting the numbers is necessary, not sufficient. Each task is promoted separately, and one level at a time.

## 4. Who approves

The Team Lead and the Staff SRE, both in writing. Reviewers who scored the evaluations cannot be the only approvers. Re-review is required after any tool, source or workflow change, and after an incident where AI output contributed to a wrong decision. Demotion to L1 can be ordered by either approver or the incident commander immediately.

## 5. Day-one level for this team

**L1 for every task.** Two of the six engineers are new to the codebase, so they cannot yet reliably catch a plausible but wrong claim about this system. Conditions:

- A new engineer may use the tool, but may not be the sole verifier of a brief shared outside the rotation.
- While they are new, a brief goes out only after review by one of the four experienced engineers or the incident commander.
- New engineers' reviews are not counted toward the 30-evaluation promotion evidence until they have completed their onboarding review.

Nothing here has been run on real incidents, so the 30-review baseline starts at zero.
