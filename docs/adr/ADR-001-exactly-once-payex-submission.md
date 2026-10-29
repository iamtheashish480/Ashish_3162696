# ADR-001: Reliable Payment Submission to PayEx with a Transactional Outbox

## Status

Accepted

## Context and Problem Statement

The planned production deployment targets approximately 40,000 invoices per month, with 60% arriving in the final three days of the month and a peak of approximately 80 approvals per minute. The planned integration with PayEx has a 30-second timeout, and a timeout is ambiguous: the payment may have been processed or dropped. SOC 2 Type II requires a seven-year non-repudiation audit trail.

How can the design reliably deliver approved invoices to PayEx while preventing duplicate payments and minimizing lost transactions?

## Considered Options

1. Client/API-Side Idempotency Key with Deduplication Table
2. Transactional Outbox Pattern with Reliable Worker & Service Bus
3. Optimistic Immediate Submission with Nightly Batch Reconciliation

## Decision

Adopt **Option 2: Transactional Outbox Pattern** with a deterministic idempotency key: `sha256(invoice_id + approved_timestamp + amount)`.

When an invoice is approved:

1. In a single Azure SQL ACID transaction:
   - Update `Invoice.Status` to `Approved`.
   - Insert a row into `ApprovalAuditLog`.
   - Insert a message into `PaymentOutbox` with `Status = 'Pending'`, `Payload`, and `IdempotencyKey`.
2. A background worker picks up pending outbox records, marks them `InFlight`, and invokes PayEx with the idempotency key.
3. If PayEx returns `200 OK` or `409 Conflict` (already completed), mark `PaymentOutbox` as `Completed`.
4. If PayEx times out after 30 seconds, query the PayEx Status API using the deterministic key before retrying.

## Stress Testing and Analysis

- **3:00 AM month-end failure scenario:** If PayEx goes down or starts timing out during the month-end peak of 80 approvals per minute, the web UI remains responsive because transactions only write to Azure SQL. The `PaymentOutbox` table buffers up to 25,000 rows without dropping approvals. When PayEx is restored, rate-limited worker threads drain the backlog without overwhelming the gateway.
- **Cost to build and run:**
  - Build: Two sprints for the Outbox schema, polling worker or Service Bus trigger, dead-letter queue, and health telemetry.
  - Run: Approximately $140 per month for Azure SQL Standard S2 and Azure Service Bus Standard.

## AI-Assisted Design Review

1. **Naive distributed transactions:** The agent initially proposed using a two-phase commit or distributed transaction across HTTP to PayEx and Azure SQL, which is not supported across those boundaries.
2. **Ignoring ambiguous timeouts:** The agent recommended treating HTTP 504 responses or timeouts as failed calls and immediately retrying without querying PayEx's idempotency endpoint, risking duplicate debit charges.
3. **Overestimating Azure Service Bus deduplication:** The agent suggested relying exclusively on Service Bus 10-minute duplicate detection, without accounting for outages lasting longer than 10 minutes.