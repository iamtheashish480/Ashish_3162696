# Review Evaluation: 12 cases, 3 baseline runs + 1 changed run

Raw per-case rows: [results.csv](results.csv). The README prompt is the baseline (`v1`). Every case in every run was reviewed by a fresh, isolated reviewer given only the prompt and one diff file. Runs 1-4 used `gpt-5.6-luna` (reasoning effort low), the default model of the `explore` sub-agent; run 5 used `claude-sonnet-5`. Both are taken from the session record. The main session around them ran `claude-sonnet-5.5`.

## Ground truth (my judgement; the repo does not record it)

| Defect (9) | Case |
| --- | --- |
| null `Ids` crash | 01 |
| SQL injection | 02 |
| `ToZero` rounding change | 03 |
| Delete without `RequireRole` | 05 |
| Logs invoice and session | 07 |
| `Dictionary` replaces `ConcurrentDictionary` | 08 |
| README says sign out/in resets data; nothing does that | 09 |
| Catch-all returns 200 | 10 |
| Architecture guard passes vacuously (compiler drops unused references) | 11 |

Clean (3): 04 (pure rename), 06 (added tests), 12 (the project-file test that fixed case 11). Case 09 is the least certain label.

Cases 11 and 12 are mine, from a real defect found in this session. Both are reconstructions; case 11 presents the original test as if added in a diff.

## Results

| Run | Prompt | Caught | Missed | False alarms |
| --- | --- | ---: | ---: | ---: |
| 1 | v1 | 8/9 | 1 (11) | 0/3 |
| 2 | v1 | 8/9 | 1 (11) | 0/3 |
| 3 | v1 | 7/9 | 2 (09, 11) | 0/3 |
| **Baseline total** | v1 | **23/27 = 85%** | 4/27 = 15% | **0/9 = 0%** |
| 4 | v2 checklist | 8/9 = 89% | 1 (09) | 2/3 = 67% |
| 5 | v1, **different model** (`claude-sonnet-5`) | 8/9 = 89% | 1 (11) | 1/3 = 33% (borderline) |

Case 09 was caught twice and missed once under v1, so a single pass can mislead. Case 11 was missed in all three baseline runs; in run 3 the reviewer pointed at the same function for a wrong reason (transitive references), which I scored as missed.

## The one change

Appended to the prompt: check each changed hunk for "could it compile and still be wrong?", "can a test or guard actually fail?", and "is every documentation claim true?". Model and cases unchanged.

## Run 5: a second model, same prompt

Run 5 repeats the original prompt on `claude-sonnet-5`. It catches 8 of 9 defects, the same as a typical baseline run, and misses case 11 as every v1 run did. Its one false alarm (case 12) is a portability remark about how the test locates the project file; like the run-4 false alarms I counted it under the README rule, but it is borderline. One run of one model cannot show a difference from `gpt-5.6-luna`: 8/9 against 23/27 is within noise. Both models missed the same case, which suggests case 11 is hard for reviewers that only see the diff.

## Was it an improvement?

**No clear improvement. Treat it as unproven.**

- Defect detection moved from 85% to 89%: one extra catch (case 11) out of 9.
- False alarms went from 0 of 9 clean reviews to 2 of 3. Both are borderline (a test-coverage nitpick on case 06; a hypothetical `Directory.Build.props` on case 12, which does not exist). Under the README's strict rule I counted them. If you excuse both, v2 is slightly better.
- Case 09 was missed in run 4.

**Confidence: low.**

- Run 4 is one pass, and the baseline itself varied by a case between runs.
- With 27 baseline defect reviews the 95% interval is roughly 68-94%. For 0 of 9 false alarms the upper bound is about 30%. Run 4's 2 of 3 has a very wide interval.
- The gain is one case-run, within noise.
- The prompt was written after I knew case 11, so the one improvement is the one my change targeted. That is not independent evidence.

To tell these apart: at least 3 runs of v2, and 30 or more cases with several clean ones per defect.

## Caveats

- Run 5 has no repeat, so no run-to-run variance for the second model. The single-change experiment is still run 4 only.
- The previous `EVALUATION_REPORT.md` and `results-template.csv` listed GPT-4o results I could not trace to any run; I removed them. `results-template.csv` is now header-only.
- Reviewers each received only their file, but isolation cannot be proven from the repository.
- I did the scoring and wrote two of the cases.


