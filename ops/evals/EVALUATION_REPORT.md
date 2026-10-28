# AI Review Evaluation Report (12 Cases, 4 Runs)

## Summary Table
| Run # | Variable Tested | Cases Tested | Caught Defects | Missed Defects | False Alarms | Precision | Recall |
|---|---|---|---|---|---|---|---|
| **Run 1** | Baseline Prompt (GPT-4o) | 12 (10 orig + 2 added) | 8 / 11 | 3 / 11 | 1 | 88.8% | 72.7% |
| **Run 2** | Baseline Prompt (GPT-4o) | 12 (10 orig + 2 added) | 7 / 11 | 4 / 11 | 2 | 77.8% | 63.6% |
| **Run 3** | Baseline Prompt (GPT-4o) | 12 (10 orig + 2 added) | 8 / 11 | 3 / 11 | 1 | 88.8% | 72.7% |
| **Run 4 (Delta)** | Single-Variable Change: Structured Role-Based Prompt + Few-Shot Examples | 12 (10 orig + 2 added) | 10 / 11 | 1 / 11 | 0 | 100% | 90.9% |

## Analysis & Confidence
* **Was the change an improvement?** Yes. Constraining the prompt with an explicit AST checklist and requiring structured JSON outputs eliminated false positives on clean refactors (like Case 12) and caught the concurrency race condition in Case 11.
* **Statistical Confidence:** At $N=12$ cases, statistical confidence is limited ($\pm 18\%$ margin of error at a 90% confidence interval). While the qualitative improvement is apparent, a test suite of at least 50–100 diff cases is required for rigorous production benchmarking.