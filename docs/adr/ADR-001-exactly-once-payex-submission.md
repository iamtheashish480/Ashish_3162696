# ADR-001: Guaranteed Exact-Once Submission to PayEx Payment Gateway

## Status
Accepted

## Context & Problem Statement
The Invoice Approval system processes ~40,000 invoices/month, with 60% arriving in the final 3 days of the month (peaking at ~80 approvals/minute). In January, the service integrates with PayEx. PayEx times out after 30 seconds, and timeouts are ambiguous (the payment could have been processed or dropped). SOC 2 Type II requires a 7-year non-repudiation audit trail. How do we guarantee approved invoices reach PayEx exactly once without double payments or lost transactions?

## Considered Options
1. Client/API-Side Idempotency Key with Deduplication Table
2. Transactional Outbox Pattern with Reliable Worker & Service Bus
3. Optimistic Immediate Submission with Nightly Batch Reconciliation

## Decision
Adopt **Option 2: Transactional Outbox Pattern** with a deterministic Idempotency Key (`sha256(invoice_id + approved_timestamp + amount)`).

When an invoice is approved:
1. Inside a single Azure SQL ACID transaction:
   - Update `Invoice.Status = Approved`
   - Insert row into `ApprovalAuditLog`
   - Insert message into `PaymentOutbox` (`Status = 'Pending'`, `Payload`, `IdempotencyKey`)
2. A background worker picks up `Pending` outbox records, marks them `InFlight`, and invokes PayEx with the Idempotency Key.
3. If PayEx returns 200 OK or 409 Conflict (already completed), mark `PaymentOutbox` as `Completed`.
4. If PayEx times out (30s), query the PayEx Status API using the deterministic key before initiating a retry.

## Stress Testing & Analysis
* **3:00 AM Month-End Failure Scenario:** If PayEx goes down or starts timing out during month-end peak (80 approvals/min), the web UI remains completely responsive because transactions only write to Azure SQL. The `PaymentOutbox` table buffers up to 25,000 rows without dropping approvals. When PayEx restores, rate-limited worker threads drain the backlog safely without hammering the gateway.
* **Cost to Build & Run:** 
  - Build: 2 sprints (Outbox schema, polling worker/Service Bus trigger, dead-letter queue, health telemetry).
  - Run: ~$140/month (Azure SQL Standard S2 tier + Azure Service Bus Standard tier).

## Where the Agent Was Wrong or Useless
1. **Naive Distributed Transactions:** The agent initially proposed using a two-phase commit / distributed transaction across HTTP to PayEx and Azure SQL, which is technically impossible and violates network partition tolerance.
2. **Ignoring Ambiguous Timeouts:** The agent recommended treating HTTP 504 / timeout as a failed call and immediately retrying without querying PayEx's idempotency endpoint, which would trigger duplicate debit charges on customers.
3. **Overestimating Azure Service Bus Deduplication:** The agent suggested relying exclusively on Service Bus 10-minute duplicate detection, failing to account for outages lasting longer than 10 minutes.