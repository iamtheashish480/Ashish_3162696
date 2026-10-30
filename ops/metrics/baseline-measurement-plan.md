# Baseline measurement plan

Capture before the rotation starts, so any later change can be compared against it.

## The three metrics, with sources

| Metric | Definition | Exact source / query | Available today? |
| --- | --- | --- | --- |
| Lead time for changes | Commit time to running in production | Commit time: `git log --format="%H %aI"`. Deploy time: the deployment event timestamp (GitHub: `GET /repos/{owner}/{repo}/deployments`, or the deploy job's finish time). Per change: deploy time minus the author time of the first commit in the PR. Report the median and p90. | **Not in full.** There is no deployment, and the repo has 2 commits. Only the CI part is measurable (below). |
| Change failure rate | Deployments that cause a rollback, hotfix or incident / all deployments | Numerator: incidents or rollbacks linked to a deployment (incident tracker, label `caused-by-deploy`). Denominator: `GET /repos/{owner}/{repo}/deployments` count. | **Not today.** No deployments exist. The CI failure rate is a different, weaker measure (below). |
| Review turnaround | PR opened to first review | `GET /repos/{owner}/{repo}/pulls?state=all`, then `GET .../pulls/{n}/reviews`. Value: first `submitted_at` minus PR `created_at`. Median and p90. | **No.** The repo has no pull request history (2 commits, no review data), and `gh` is not installed here. |

## Captured today (from `ops/pipeline/run-timings.csv`, 20 CI runs, 2026-01-19 to 2026-02-02)

Method: sum `duration_seconds` per `run_number` (one sequential job, so the sum is wall time). Reproduce with `Import-Csv ops\pipeline\run-timings.csv | group run_number`, then sum `duration_seconds`.

### 1. CI duration: the pipeline part of lead time

| Statistic | Minutes |
| --- | ---: |
| Mean | **31.3** |
| Median | **31.3** |
| p90 | 34.5 |
| Min / max | 28.4 / 35.1 |

Largest step by mean: static analysis, 8.3 minutes. Next: end-to-end tests 3.7, container build 2.8, integration tests 2.4.

This is a floor under lead time, not lead time itself: it excludes waiting for review and deployment.

### 2. CI failure rate: a proxy, not change failure rate

- Failed runs: **2 of 20 (10%)**, runs 6 and 14, both on pull requests.
- Pull requests: 2 of 12 failed. Pushes to `main`: 0 of 8 failed.
- A failed run here means a check failed before merge, so it is not a production failure. Treat it as a signal of how often changes arrive broken, and do not report it as DORA change failure rate.
- Sample is small: a 95% interval for 2 of 20 is roughly 3-30%.

## Correction to the earlier plan

The previous version of this file reported a mean pipeline duration of **11.85 minutes** and a "median pull request turnaround of 27.4 hours" from "GitHub Actions run logs". Neither number reproduces: the CSV gives 31.3 minutes, and the repository has no pull request data to produce the 27.4 hours. Both were removed.

## Still to capture before the rotation

1. Enable deployment events (or record deploys in a log) so lead time and change failure rate can be computed.
2. Install `gh` or script the REST calls above, and pull the review turnaround for the last 90 days once real PRs exist.
3. Agree what counts as a failure (rollback, hotfix within 24 hours, or an incident) and record the definition here.
4. Re-capture these numbers at the end of the first rotation, using the same queries.
