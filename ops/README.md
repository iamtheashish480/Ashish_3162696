# Operations material

Evidence packs and artefacts for working on this service the way you would in
production. Nothing here runs as part of the application or its tests.

| Folder | What it holds |
| --- | --- |
| `incidents/INC-2041` | Approvals slow and failing. Logs, metrics and the deploy diff for a completed incident window. |
| `incidents/INC-2047` | Approved invoices not triggering notifications. A live incident, read-only output only. |
| `pipeline` | The service's full CI pipeline and per-step timings for the last 20 runs. |
| `evals` | Twelve fixed review cases, recorded results for five runs, and the evaluation report. |

## A note on the scenario

The invoice approval service in this repository is an internal tool. The
material here assumes the version of it that is going to production: a
24x7 on-call rotation, a database, a message topic, and an integration with
an external payment provider called PayEx. None of that is in the code yet,
which is the point — the diagrams, decisions and policies have to be made
before it is.

Names, hosts and figures are invented. No real system, customer or vendor
appears anywhere in this folder.

## No answers here

These packs deliberately do not state what the root cause is, which cases
contain defects, or which pipeline changes are safe. Working that out is the
exercise. If you are facilitating rather than participating, the answers are
in the session runbook, not in the repository.

## Agent setup (the setup the risk audit covers)

The work on this repository is done with an AI coding assistant in VS Code (Copilot SDK) running in a single local session. It has:

- **File tools:** read, search, create and edit files anywhere in the working tree.
- **Shell:** PowerShell on the engineer's machine, including `git` and `curl`, with the engineer's environment and network access.
- **Web:** `web_fetch` for arbitrary URLs.
- **MCP servers:** GitHub (repository, issue and pull request read access), GitKraken (git and pull request actions), Figma, and draw.io (diagram generation).
- **Sub-agents:** read-only `explore`, `task` and `general-purpose` agents it can launch.

It reads untrusted material (issue and PR text, diffs, incident logs, web pages) and the private working tree in the same session, and can act outward (fetch, push, comment). The audit of this setup is in [audits/lethal-trifecta-risk-audit.md](audits/lethal-trifecta-risk-audit.md).

