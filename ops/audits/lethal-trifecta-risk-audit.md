# Risk Audit: The Lethal Trifecta in CI/CD Automation

## 1. Trifecta Identification in Current ops/README.md Setup
The "Lethal Trifecta" occurs when three dangerous system properties overlap:
1. **Untrusted Input:** The PR reviewer agent accepts untrusted Markdown and pull request code from public or semi-trusted contributors.
2. **Broad Tool Access:** The agent environment possesses read/write access to repository files, CI pipeline secrets, and deployment triggers.
3. **Automated Execution / Autonomous Action:** The agent is authorized to automatically merge PRs or push commits back to `main` when checks pass.

**Verdict:** All three legs are currently present in the autonomous pull request triage proposal.

## 2. Redesign: Breaking the Cycle
We dismantle the trifecta by eliminating **Automated Execution**:
* **Read-Only Agent Sandbox:** The AI review agent runs in an isolated runner with no repository write tokens or production credentials.
* **Mandatory Human Gate:** The agent is permitted only to emit structured review comments. It cannot approve pull requests or trigger deployments. Two human code owners must physically click "Approve" before merges can proceed.