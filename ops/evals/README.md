# AI Review Evaluation

Twelve fixed cases for measuring how well a review prompt performs on this
codebase. Each case is a diff in `cases/`. Some contain a defect and some do
not; which is which is not recorded here.

## Why run it

A review that finds something on one pull request has told you nothing about
how it behaves in general. This set gives you three numbers you can compare
over time, across models and across prompt versions:

- **Caught** — the case has a defect and the review named it.
- **Missed** — the case has a defect and the review did not.
- **False alarm** — the case is clean and the review reported a defect anyway.

The false alarm rate is the one people forget to measure, and it is the one
that decides whether anyone keeps using the reviewer.

## How to run it

1. Start a fresh session for each case. Context from a previous case will
   change the answer.
2. Give the agent the case diff and the review prompt below. Nothing else.
3. Record what it reported in `results.csv`, using `results-template.csv` as
   the starting point.
4. Run the full set three times with the baseline configuration. Output varies
   between runs, so a single pass gives you an anecdote rather than a rate.
5. For a fourth run, change exactly one variable — such as the prompt, model,
   or context file — and run the full set again.

## The review prompt

> Review this diff for defects. For each finding give the file, the line, what
> goes wrong, and how confident you are. If you find nothing, say so
> explicitly rather than listing general observations.

## Scoring

Score one case per row. A finding counts as caught only if it names the actual
defect; a vague note that something "could be improved" near the right line
does not count, and neither does a correct finding reached for the wrong
reason. Be strict about this, because a generous score here is a score you
cannot act on later.

For the clean cases, style suggestions are not false alarms. A false alarm is
the review asserting a defect that is not there.

## Recorded results

[results.csv](results.csv) holds the recorded runs, one row per case per run (60 rows). Columns: `run`, `case`, `model`, `prompt_version`, `verdict` (`caught`, `missed`, `clean` or `false_alarm`), `finding_summary`, `confidence_reported`, `notes`.

| Run | Model | Prompt |
| --- | --- | --- |
| 1-3 | gpt-5.6-luna | v1 (the prompt above) |
| 4 | gpt-5.6-luna | v2_checklist (v1 plus a paragraph on compile-but-wrong code, guards that can fail, and documentation claims) |
| 5 | claude-sonnet-5 | v1 |

Cases 11 and 12 were added by the author (11 defect, 12 clean). `results-template.csv` is the header row only. Analysis and confidence are in [EVALUATION_REPORT.md](EVALUATION_REPORT.md).

