# Risk audit: the lethal trifecta in the agent setup

## Scope, and a gap to resolve

The brief refers to an agent setup "at the end of `ops/README.md`". That file ends with the sections "A note on the scenario" and "No answers here"; it describes no agent setup. The earlier version of this audit audited a "pull request reviewer that merges to `main`", which no file in the repository describes, so it was withdrawn.

This audit covers the only agent setup that provably exists: the coding agent used on this repository (Copilot CLI in VS Code), with its shell, file edit, web fetch, and the GitHub, GitKraken, Figma and draw.io MCP servers. **If you meant a different setup, give me its description and I will redo section 1; the redesign pattern will carry over.**

## 1. Which legs are present

| Leg | Present? | Evidence |
| --- | --- | --- |
| Access to private data | **Yes** | The agent reads the whole working tree, git history, `ops/incidents` logs, and the user's environment variables and files via the shell. The GitHub MCP reads private repos. |
| Exposure to untrusted content | **Yes** | It reads web pages (`web_fetch`), issue and PR bodies and comments (GitHub MCP), diffs from contributors (the review cases), and incident logs, which can contain attacker-controlled strings (INC-2041 and INC-2047 hold request data). |
| Ability to communicate externally / act outward | **Yes** | `web_fetch` makes outbound requests, the shell can run `curl`, and the git and GitHub tools can push, comment, and open PRs. |

All three are present in a single session, so exfiltration by prompt injection is possible: a poisoned page, issue or log line could instruct the agent to read a secret and send it out in a URL or a comment. I did not test an attack; this is a structural finding, not a demonstrated exploit.

What I could verify: this repository contains no real credentials (AGENTS.md rule), only demo accounts. That limits the first leg for the repo itself, not for the user's machine.

## 2. Decision: redesign, removing the outward leg

Removing the untrusted-input leg is not realistic (the work is reading issues, logs and diffs). Removing private data is not possible either. The cheapest leg to break is **external communication**:

1. **Split the sessions by trust.** Sessions that read untrusted content (triage of issues, logs, external diffs) run with web fetch, `curl`, push and the GitHub write tools **disabled**; they can only write local files and post nothing.
2. **Writing sessions never read untrusted content.** Sessions with push and comment rights work from reviewed, in-repo instructions only.
3. **A human carries output across.** Output from the read-only session is copied into the writing session by a person, who reads it first.
4. **Least-privilege tokens.** The GitHub MCP token is read-only for untrusted sessions. No cloud or payment credentials are present on the machine for either (PayEx does not exist yet).
5. **Network egress** for the shell is limited to the package and git hosts where the platform allows it.

Control 1 is a configuration of tool permissions, so it is checkable: list the enabled tools at session start and record them in the PR description.

## 3. What would make me revisit

- Any real secret, token or customer data enters the repo or the machine used for agent sessions (e.g. when the PayEx key appears).
- The agent is given an always-on trigger (e.g. summaries auto-posted on an alert), which would remove the human carrying output across.
- A session needs both web access and write access (an exception request).
- The first outbound-capable MCP server is added.
- Any prompt-injection incident or near miss, even a harmless one.
- Ownership: the platform owner approves exceptions; the on-call lead approves any change to the control list.

## 4. Residual risk, accepted in writing

A human can still paste poisoned text from the read-only session into the writing session. I accept this because the human reads it first. It is a weaker control than a technical one, and the plan is to review it after the first month of the rotation.
