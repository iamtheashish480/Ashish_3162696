Invoice Approval Service — Tier-2 Production Readiness

A .NET 8 invoice approval service focused on architectural boundaries, reliable payment integration, operational readiness, security, and responsible AI-assisted software development.

This repository contains the application code, architecture governance tests, architecture documentation, operational risk audits, AI code-review evaluation suite, baseline engineering metrics, and human-in-the-loop AI augmentation audit logs.

Key Features
.NET 8 Application: Invoice approval API and domain logic.
Architecture Governance: Automated tests for domain dependency boundaries and entity state encapsulation.
Architecture Documentation: As-built and target-state diagrams with source citations and documented inferences.
Payment Reliability: Transactional Outbox design, idempotency, retry handling, and payment reconciliation considerations.
Operational Readiness: Incident-response autonomy policy and baseline engineering metrics.
Security: Lethal trifecta risk assessment and safeguards against unsafe autonomous execution.
AI Evaluation: Twelve diff cases evaluated across four runs.
Auditability: Human-reviewed records of AI suggestions, decisions, limitations, and verification activities.
Repository Structure
text
.
├── backend/
│   ├── InvoiceApp.sln
│   ├── src/
│   │   ├── InvoiceApp.Api/
│   │   └── InvoiceApp.Domain/
│   └── tests/
│       └── InvoiceApp.Tests/
├── docs/
│   ├── architecture/
│   │   └── as-built-and-target-diagrams.md
│   ├── adr/
│   │   └── ADR-001-exactly-once-payex-submission.md
│   └── policies/
│       └── autonomy-policy-incident-response.md
├── ops/
│   ├── evals/
│   │   ├── cases/
│   │   ├── results-template.csv
│   │   └── EVALUATION_REPORT.md
│   ├── audits/
│   │   └── lethal-trifecta-risk-audit.md
│   └── metrics/
│       └── baseline-measurement-plan.md
├── audit-logs/
├── demo/
└── README.md


Note: This is the expected logical structure. Verify directory and file names against the actual repository before submission.

Deliverables and Documentation
Deliverable	Location
Architecture diagrams and citations	docs/architecture/as-built-and-target-diagrams.md
Architecture governance tests	backend/tests/InvoiceApp.Tests/ArchitectureTests.cs
PayEx integration ADR	docs/adr/ADR-001-exactly-once-payex-submission.md
Incident-response autonomy policy	docs/policies/autonomy-policy-incident-response.md
AI review evaluation suite	ops/evals/
Lethal trifecta risk audit	ops/audits/lethal-trifecta-risk-audit.md
Baseline measurement plan	ops/metrics/baseline-measurement-plan.md
AI augmentation audit logs	audit-logs/
Prerequisites
.NET 8 SDK
Git
Required application configuration and external credentials, if applicable

Verify the installed .NET SDK:

bash
dotnet --info

Quickstart
1. Clone the repository
bash
git clone https://github.com/iamtheashish480/Ashish_3162696.git
cd Ashish_3162696

2. Restore dependencies
bash
dotnet restore backend/InvoiceApp.sln

3. Build the solution
bash
dotnet build backend/InvoiceApp.sln

4. Run the tests
bash
dotnet test backend/InvoiceApp.sln


This command executes the automated tests included in the solution, including the architecture governance tests.

All tests should pass before submission. If a test fails, investigate and fix the underlying issue rather than weakening the architectural rule without justification.

5. Run the API
bash
dotnet run --project backend/src/InvoiceApp.Api


Use the local URL printed in the terminal to access the API. Configure any required application settings, connection strings, or external credentials before starting the service.

Press Ctrl+C to stop the application.

Architecture and Governance

The project follows a domain-centered architecture intended to keep core business rules independent of API and infrastructure concerns.

Architecture governance tests validate:

Dependency boundaries between the domain and other application layers.
Encapsulation of core invoice state.
Compliance with the architectural rules implemented in the test project.

Core domain state should be changed through appropriate domain operations rather than unrestricted public setters. For example:

csharp
public InvoiceStatus Status { get; private set; }


The actual implementation must ensure that constructors, domain methods, persistence mapping, and application workflows remain compatible with the chosen encapsulation strategy.

Architecture documentation distinguishes the current implementation from proposed target-state components.

See docs/architecture/as-built-and-target-diagrams.md for the diagrams, source references, and documented inferences.

PayEx Integration and Reliability

The payment integration design is documented in docs/adr/ADR-001-exactly-once-payex-submission.md.

Transactional Outbox

The proposed Transactional Outbox pattern records outbound payment work durably alongside the relevant business transaction. A background processor can then retrieve pending messages, submit payment requests, retry eligible failures, and record processing outcomes.

The intended design considers:

Durable recording of payment requests.
Stable idempotency keys, where supported by the gateway.
Controlled retries and backoff.
Concurrent message processing.
Handling of ambiguous network timeouts.
Reconciliation of unresolved payment outcomes.
Monitoring of processing latency and pending-message backlog.
Delivery Guarantees

The Outbox pattern supports reliable message processing, but it does not independently guarantee exactly-once payment execution.

Network timeouts may leave the payment outcome unknown. Duplicate business effects can be prevented only when the integration's transaction boundaries, deduplication strategy, and gateway-supported idempotency behavior work together correctly.

The ADR addresses the target workload of 80 approvals per minute, failure scenarios, peak-load considerations, and limitations of AI-generated recommendations.

Production capacity and payment guarantees should be claimed only when supported by the actual implementation, gateway contract, and recorded verification results.

Operational Readiness and Security
Incident-Response Autonomy

Policy: docs/policies/autonomy-policy-incident-response.md

The initial operating model uses Day-One Level 1 assistive autonomy for incident-log triage.

AI may summarize incident logs, organize evidence, and recommend investigative steps. Human operators remain responsible for validating findings and approving consequential actions.

This approach supports gradual adoption while accounting for team familiarity with the codebase and operational risk.

Lethal Trifecta Risk Audit

Audit: ops/audits/lethal-trifecta-risk-audit.md

The audit examines the combination of:

Access to untrusted inputs.
Access to sensitive information or privileged resources.
The ability to execute actions or communicate with external systems.

Mitigations emphasize restricted permissions, separation of analysis from execution, and human approval for consequential operations. Security controls should be enforced through actual system permissions and execution boundaries, not solely through AI instructions.

AI Review Evaluation

The evaluation suite is maintained in ops/evals/ and contains twelve diff cases evaluated across four runs.

Key artifacts:

ops/evals/cases/
ops/evals/results-template.csv
ops/evals/EVALUATION_REPORT.md

The evaluation assesses defect detection, false alarms, missed defects, and the reliability of AI-assisted code review.

The report should calculate metrics from individual case outcomes:

True positives (TP): Defects correctly identified.
False positives (FP): Incorrect defect findings.
False negatives (FN): Existing defects that were missed.
True negatives (TN): Clean cases correctly identified as not containing a defect.

Precision and recall are calculated as follows:

Precision
=
𝑇
𝑃
𝑇
𝑃
+
𝐹
𝑃
Recall
=
𝑇
𝑃
𝑇
𝑃
+
𝐹
𝑁

When a denominator is zero, the report should document the metric as undefined rather than silently reporting 100%.

Evaluation conclusions should reflect the underlying case-level results and acknowledge the limitations of the test dataset.

Baseline Engineering Metrics

The baseline measurement plan is available at ops/metrics/baseline-measurement-plan.md.

Metric	Reported Baseline
Average CI duration	11.85 minutes
Pull-request turnaround	27.4 hours

These are documented baseline measurements. Future comparisons should use consistent definitions, measurement periods, and source data.

The measurement plan covers engineering delivery metrics and should specify the data sources and calculation methods used for each metric.

AI Augmentation Audit Logs

The audit-logs/ directory contains chronological records of AI-assisted engineering activities, including:

Task context and requested work.
AI-generated suggestions and proposed changes.
Tools and skills used.
Human acceptance, rejection, or revision of suggestions.
Verification activities and observed results.
Known limitations and required rework.
Related tasks and affected artifacts.

These records support traceability and demonstrate human oversight throughout AI-assisted software development.

An audit entry documents the verification performed at that point; it does not, by itself, establish overall production readiness.

Demo Video

Expected location: demo/demo-video.mp4

Target duration: Approximately 8–10 minutes.

The demonstration should cover:

As-built and target-state architecture diagrams, including citations and inferred components.
Execution of the architecture governance tests.
The PayEx integration ADR, Transactional Outbox design, timeout ambiguity, peak-load considerations, and AI limitations.
The incident-response autonomy policy and Day-One Level 1 decision.
The twelve-case AI review evaluation suite, four runs, and statistical findings.
The lethal trifecta risk audit and baseline engineering metrics.
AI augmentation audit logs, including human decisions and verification outcomes.

Verify that the video exists at the documented path before submission.

Final Verification Checklist

Before submission, confirm that:

The solution restores and builds successfully.
All automated tests pass.
Architecture diagrams match the current implementation and distinguish proposed components.
Evaluation metrics match the recorded case-level results.
Payment reliability claims match the actual implementation and gateway capabilities.
Baseline measurements can be traced to their source data.
Audit logs are present, readable, and consistent.
All required documentation is present.
The demo video exists at the documented location.
Project Objective

Demonstrate how architectural governance, reliable integration design, operational safeguards, and human-supervised AI assistance can support maintainable and responsible software delivery.