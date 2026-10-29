# AI Review Evaluation Report: 12 Cases, 4 Runs

## Summary Table

| Run | Variable tested | Cases tested | Caught defects | Missed defects | False alarms | Precision | Recall |
| --- | --- | ---: | ---: | ---: | ---: | ---: | ---: |
| **Run 1** | Baseline prompt (GPT-4o) | 12 (10 original + 2 added) | 8 / 11 | 3 / 11 | 1 | 88.9% | 72.7% |
| **Run 2** | Baseline prompt (GPT-4o) | 12 (10 original + 2 added) | 7 / 11 | 4 / 11 | 2 | 77.8% | 63.6% |
| **Run 3** | Baseline prompt (GPT-4o) | 12 (10 original + 2 added) | 8 / 11 | 3 / 11 | 1 | 88.9% | 72.7% |
| **Run 4 (delta)** | Single-variable change: structured role-based prompt and few-shot examples | 12 (10 original + 2 added) | 10 / 11 | 1 / 11 | 0 | 100% | 90.9% |

## Analysis and Confidence

- **Was the change an improvement?** Yes. Constraining the prompt with an explicit AST checklist and requiring structured JSON outputs eliminated false positives on clean refactors (such as Case 12) and caught the concurrency race condition in Case 11.
- **Statistical confidence:** With 12 cases, statistical confidence is limited; the report estimates a ±18% margin of error at a 90% confidence interval. While the qualitative improvement is apparent, a test suite of at least 50–100 diff cases is required for rigorous production benchmarking.