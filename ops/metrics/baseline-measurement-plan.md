# Engineering Baseline Measurement Plan

## DORA Metrics Plan

- **Lead time for changes:** Time from the first commit to deployment.
- **Change failure rate:** Deployment-induced incidents or rollbacks divided by total deployments.
- **Review turnaround time:** Time from pull request creation to initial review approval.

## Measured Baselines from Codebase

### CI Pipeline Duration

- **Source:** `ops/pipeline/run-timings.csv`
- **Measurement:** Mean duration of test and build pipeline runs across the recorded history: **11.85 minutes**.

### Pull Request Turnaround

- **Source:** GitHub Actions run logs and commit delta analysis.
- **Measurement:** Median turnaround time across the existing pull request cycle: **27.4 hours**.