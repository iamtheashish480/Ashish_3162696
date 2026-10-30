# ADR-001: Reliable PayEx Submission with a Transactional Outbox

## Status

Accepted as the proposed target-state design. Production implementation, provider capabilities, capacity, and cost estimates remain to be verified. The current demo application does not integrate with PayEx.

## Context and Problem Statement

The planned production scenario assumes approximately 40,000 invoices per month, with 60% arriving in the final three days and a peak of approximately 80 approvals per minute. PayEx is assumed to have a 30-second request timeout, but a timeout cannot establish whether a payment was processed. The scenario also requires a seven-year audit trail.

The goal is to ensure that every approved payment intent is durably recorded and that retries do not create duplicate payment effects. A network protocol alone cannot guarantee exactly-once delivery. Exactly-once **payment effect** is achievable only if the provider supports durable idempotency for the full retry and reconciliation period, or an equivalent deduplication contract. Without that provider guarantee, ambiguous outcomes must be reconciled rather than blindly retried.

## Options Considered

| Option | Benefits | Trade-offs and failure modes |
| --- | --- | --- |
| **1. Client/API idempotency key with a deduplication table** | Prevents duplicate approval requests when the same client request is retried; can reject repeated request keys. Less infrastructure than a separate worker and queue. | A local deduplication table cannot atomically commit with PayEx. If the process crashes after PayEx accepts a payment but before the local result is saved, the outcome is still ambiguous. It needs durable request state, recovery, and PayEx-side idempotency to protect the external payment effect. |
| **2. Transactional Outbox with a worker** | Stores the approved state and payment intent in one database transaction. A worker can retry durable pending work, track outcomes, and recover after process or PayEx outages. | Adds schema, worker operations, retry/lease handling, monitoring, and reconciliation. Delivery is at least once; duplicate external effects are prevented only by PayEx idempotency or an equivalent provider-side deduplication contract. |
| **3. Immediate submission with nightly reconciliation** | Fastest initial implementation; avoids an outbox worker and broker at first. Reconciliation can identify some missing or unresolved submissions. | Approval depends on a fragile synchronous call, while timeouts still leave outcomes unknown. Nightly detection delays recovery, and reconciliation cannot by itself prevent duplicate charges or guarantee that every intent is submitted. Poor fit for timely, high-volume month-end processing. |

## Decision

Choose **Option 2: Transactional Outbox with a reliable worker**, provided PayEx confirms a suitable idempotency contract before production launch. The Outbox addresses the local database-to-payment handoff; it does not, by itself, guarantee exactly-once payment execution.

For each logical payment operation:

1. In one database transaction, transition the invoice to approved, write the audit event, create a unique immutable `payment_operation_id`, and insert the corresponding pending outbox record. Enforce uniqueness for the operation so a repeated approval request cannot create a second payment intent.
2. Persist the complete payment request and a stable provider idempotency key derived from or assigned to that operation. Reuse exactly the same key and payload on every retry; do not generate a new key from a retry timestamp.
3. Have a worker claim pending records with a recoverable lease or equivalent concurrency control, submit them to PayEx, and record the provider reference and outcome. Expired claims must become eligible for recovery after a worker crash.
4. On a confirmed success, mark the operation complete. Treat a conflict as success only if PayEx documents that response as proof that the same idempotency key and request already completed; a generic `409 Conflict` is not sufficient.
5. On timeout or connection loss, mark the outcome **unknown**. Query PayEx by the stable key if its documented status API supports that lookup. Otherwise, retry only with the same key and only if PayEx guarantees deduplication for the entire retry/reconciliation horizon. If neither condition is met, quarantine the operation for human reconciliation; do not blindly submit it again.
6. Retry transient failures with bounded, rate-limited backoff. Alert on age and size of the pending/unknown backlog. Dead-lettering must preserve the payment intent and create an actionable reconciliation path; it must not silently discard work.

The key's uniqueness scope, payload matching rules, retention period, status-query semantics, and behavior for repeated or conflicting requests must be confirmed from the PayEx contract. If PayEx cannot provide durable idempotency or an equivalent guarantee, this design cannot claim exactly-once payment effects.

## Decision Challenges and Stress Tests

These are design challenges against the stated scenario, not production load-test results or evidence of PayEx behavior.

### Challenge 1: Prefer a different option

**Challenge:** Prefer Option 1 because it has fewer moving parts. Does the recommendation change?

**Assessment:** Not for the stated goal of durable, recoverable payment submission through outages. A deduplication table protects repeated API requests but cannot close the crash window between PayEx accepting a payment and the application recording that result. Option 1 would be reasonable if the requirement were limited to deduplicating client approval requests and payment submission could tolerate manual recovery. If PayEx offers strong, durable idempotency, Option 1 could be made safer, but it would still need durable dispatch and recovery logic; at that point it approaches an Outbox design. The recommendation therefore does not change solely to favor a simpler initial implementation.

### Challenge 2: PayEx fails at 3 a.m. on the last day of the month

Assume the worker cannot obtain a definitive response from PayEx during the peak of 80 approvals per minute:

- The approval transaction should continue only if it can durably commit the pending payment intent. The UI must report payment submission as pending—not imply that PayEx completed the payment.
- If PayEx is unavailable, the outbox backlog grows. At 80 new operations per minute and zero successful processing, 25,000 records would represent about 312.5 minutes of incoming work. This arithmetic is not evidence that the database, worker, or proposed buffer has been load-tested to that capacity.
- On recovery, the worker must throttle requests to documented provider limits and drain backlog faster than new work arrives. Monitor backlog age, size, retry counts, unknown outcomes, database capacity, and provider throttling.
- A timeout after PayEx may have accepted the request is an unknown outcome. Resolve it through the provider's documented status or idempotency mechanism; do not submit a new operation with a new key.
- If the database is unavailable, approval and outbox creation cannot be committed together. Fail the approval clearly rather than approving without a durable payment intent.
- Reconcile unresolved payments and missed business effects with finance. Restoring forward processing does not prove that previously missed notifications or payments were recovered.

The former 25,000-row buffer and “UI remains responsive” statements are design targets only. They require load, outage, recovery, and database-capacity tests before they can be treated as guarantees.

### Challenge 3: Cost to build and run

- **Build estimate:** Approximately two sprints for the schema, transactional approval/outbox write, worker, retry and recovery behavior, dead-letter/reconciliation workflow, monitoring, and tests. This is a planning estimate, not an engineering commitment; it excludes or may undercount production hardening, provider certification, security review, and seven-year audit retention.
- **Run estimate:** Unknown. An earlier draft quoted about $140/month for Azure SQL Standard S2 plus Azure Service Bus Standard. That figure is withdrawn: this decision uses no broker, so the Service Bus line item does not apply, and the S2 sizing was never derived from the 80/minute peak or seven-year retention. Cost drivers to price before approval: database tier and storage over seven years, worker compute, monitoring, and PayEx fees or limits. No vendor is chosen by this ADR.
- **Trade-off:** Option 2 has higher implementation and operational cost than a synchronous first version, but it makes pending work visible and recoverable. Option 3 may cost less initially but leaves longer recovery windows and unresolved payment risk. Option 1 does not remove the need for durable dispatch and recovery if PayEx calls must be reliable.

## Revisit Conditions

Reopen this decision if any of the following occurs:

- PayEx's contract does not guarantee idempotency, key retention, request matching, or status lookup for the required retry and reconciliation horizon.
- Measured traffic, outage duration, recovery throughput, database growth, or provider rate limits invalidate the proposed capacity assumptions.
- A load, failure-injection, or reconciliation test demonstrates lost intents, duplicate effects, unsafe retries, or unacceptable backlog recovery.
- The target changes to another provider, payment workflow, audit-retention requirement, or deployment architecture.
- A materially lower-risk option becomes available, or operational costs exceed the approved budget.

## Where the agent was wrong or useless

Provenance: items 5-7 are errors I can point to in this repository and session. Items 1-4 were written into this file earlier; I could not find a transcript that confirms the agent said them, so treat them as reported, not verified. The record owner should confirm or delete them.

5. **Broker contradiction (verified in this file):** the decision is an outbox plus worker that calls PayEx directly, yet the draft still credited the outbox with surviving "broker outages" and priced Azure Service Bus at about $140/month. A broker was never part of the decision. Both are corrected above.
6. **Wrong claim about this very ADR (verified):** the architecture doc said the ADR names no cloud products, but Challenge 3 named Azure SQL and Service Bus. The claim was corrected to say they appear only in the withdrawn cost estimate.
7. **Useless on cost and capacity (verified):** the agent supplied a build estimate (two sprints), a run cost and a 25,000-row buffer with no inputs behind them. Only the 312.5-minute figure is checkable: 25,000 / 80 per minute. The rest cannot be validated without PayEx limits, volumes and pricing.

1. **Naive distributed transactions:** The agent initially proposed a two-phase commit across the HTTP call to PayEx and the SQL transaction. That cannot make the remote HTTP side effect atomic with the local database transaction.
2. **Ignoring ambiguous timeouts:** The agent suggested treating HTTP 504 or a timeout as a failed payment and immediately retrying. PayEx may already have processed the request, so a new submission could cause a duplicate debit.
3. **Overestimating broker deduplication:** The agent suggested relying on Service Bus's 10-minute duplicate detection. Broker deduplication does not establish payment idempotency and cannot protect against outages or retries beyond its configured window.
4. **Overstating guarantees and capacity:** An Outbox alone cannot guarantee exactly-once payment effects, and the proposed 25,000-row buffer, responsive UI, two-sprint estimate, and $140 monthly run cost are unverified assumptions. They require provider-contract review, implementation, pricing, and failure/load testing.
