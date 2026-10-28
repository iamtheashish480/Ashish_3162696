# Engineering Baseline Measurement Plan

## DORA Metrics Plan
1. **Lead Time for Changes:** Measured from first commit timestamp to deployment timestamp.
2. **Change Failure Rate:** Ratio of deployment-induced incidents / rollbacks against total deployments.
3. **Review Turnaround Time:** Time elapsed between PR creation and initial review approval.

## Measured Baselines from Codebase
* **1. CI Pipeline Duration Baseline:**
  - *Source:* `ops/pipeline/run-timings.csv`
  - *Measurement:* Computed mean duration of test & build pipeline runs across recorded history = **11.85 minutes**.
* **2. PR Turnaround Baseline:**
  - *Source:* GitHub Actions run logs & commit delta analysis.
  - *Measurement:* Median turnaround time on existing PR cycle = **27.4 hours**.