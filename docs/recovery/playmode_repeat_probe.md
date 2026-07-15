# Recovery PlayMode Repeat Probe

This documentation-only marker proves that a bounded PlayMode harness layer remains green on an unrelated descendant commit.

It changes no Unity source, scene, asset, package, project setting or test implementation.

## R1.2 fake-XR repeat

The currently tested suite contains:

- the promoted one-frame lifecycle test;
- the tests-only tracked-head/controller/XRI composition test;
- the right-ray hover/select interaction test;
- leak-free teardown of active and partial fixtures.

Promotion of R1.2 requires this marker SHA to produce:

- three passing tests and zero failed/skipped tests;
- a successful `Recovery PlayMode Observation`;
- a test-result artifact;
- a durable observation recording this exact tested SHA.
