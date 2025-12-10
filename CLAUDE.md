# Workflow-Driven Test-Driven Development for Unity

## Overview
This project uses a **workflow-driven TDD approach** where the GitHub Actions workflow orchestrates the TDD cycle. The workflow automatically:
1. Runs PlayMode tests to determine current state
2. Determines which TDD phase you're in based on test results
3. Calls Claude with phase-specific instructions
4. Loops automatically until completion

**You don't run tests manually - the workflow does it for you between iterations.**

---

## Phase 1: Requirements Clarification

**Input:** User's feature request

**Process:**
1. Read the feature request carefully
2. Identify any ambiguities or missing details
3. If requirements are unclear:
   - **STOP** implementation
   - Reply with specific clarifying questions:
     - What are the expected inputs and outputs?
     - What are the success criteria?
     - Are there edge cases to consider?
     - What components/systems will this interact with?
   - **WAIT** for user response before proceeding
4. If requirements are clear:
   - Summarize your understanding
   - List testable acceptance criteria
   - Proceed to Phase 2

**Exit Criteria:** Clear, testable requirements documented

---

## Phase 2: Write Failing Tests

**Workflow Status:** No tests exist yet, or tests need to be written

**Your Task:**
1. Create or locate PlayMode test file in `Assets/Tests/PlayMode/`
2. Write tests that verify ALL requirements:
   - One test per acceptance criterion
   - Use descriptive test names: `When[Condition]_Should[ExpectedBehavior]`
   - Test edge cases and error conditions
   - Tests MUST fail initially (no implementation exists yet)
3. Commit with message: `test: Add PlayMode tests for [feature]`
4. Push your changes

**Example Test Structure:**
```csharp
[UnityTest]
public IEnumerator WhenPlayerCollectsItem_ShouldIncreaseScore()
{
    // Arrange - Set up test conditions

    // Act - Execute the behavior

    // Assert - Verify expected outcome
    yield return null;
}
```

**What Happens Next:**
- The workflow will automatically run your tests
- It will detect they all fail (expected!)
- It will call you again in Phase 3 to implement the code

**Critical Rules:**
- ❌ DO NOT run tests yourself with unity-editor
- ❌ DO NOT implement any production code
- ✅ DO commit and push when tests are written
- ✅ DO use descriptive test names

---

## Phase 3: Implement Minimum Code

**Workflow Status:** Tests exist and some/all are failing

**Your Task:**
1. Write the **simplest possible code** to make failing tests pass
2. Focus on making tests pass, not writing perfect code
3. Hard-code values if necessary (will refactor later)
4. Commit with message: `feat: Implement [specific functionality]`
5. Push your changes

**What Happens Next:**
- The workflow will run tests automatically
- If tests still fail: You'll be called again in Phase 3
- If all tests pass: You'll be called in Phase 4 to refactor

**Critical Rules:**
- ❌ DO NOT add features not required by tests
- ❌ DO NOT refactor yet (that's Phase 4)
- ❌ DO NOT run tests yourself with unity-editor
- ✅ DO use the simplest solution
- ✅ DO commit and push when you've made progress
- ✅ DO focus on ONE test at a time if possible

---

## Phase 4: Refactor Implementation

**Workflow Status:** All tests pass! Time to improve code quality

**Your Task:**
1. Review the implementation for:
   - Code duplication
   - Hard-coded values that should be parameterized
   - Long methods that should be split
   - Complex logic that needs simplification
   - Missing modular structure
2. Refactor to achieve:
   - **Simple:** Easy to understand and maintain
   - **Modular:** Clear separation of concerns
   - **DRY:** No unnecessary duplication
   - **SOLID:** Follow good design principles
3. Commit with message: `refactor: [description of improvement]`
4. Push your changes

**What Happens Next:**
- The workflow will run tests to ensure they still pass
- If tests pass: You'll be called in Phase 5 for final verification
- If tests fail: You'll be called in Phase 3 to fix them

**Critical Rules:**
- ❌ DO NOT add new features
- ❌ DO NOT change test behavior
- ❌ DO NOT run tests yourself with unity-editor
- ✅ DO make incremental refactors
- ✅ DO commit and push after each logical refactor
- ✅ DO focus on code quality, not new functionality

---

## Phase 5: Final Verification

**Workflow Status:** All tests pass after refactoring - you're almost done!

**Your Task:**
1. Verify all tests pass (workflow already confirmed this)
2. Review test coverage:
   - All requirements tested?
   - Edge cases covered?
   - Error conditions handled?
3. Review code quality one final time
4. If everything looks good, you're done!
5. If gaps found, add tests and push (returns to Phase 2)

**What Happens Next:**
- The workflow will attempt to create a pull request
- The TDD cycle is complete for this feature!

**Critical Rules:**
- ❌ DO NOT make changes unless you find missing tests
- ❌ DO NOT add features beyond requirements
- ✅ DO verify test coverage is complete
- ✅ DO confirm code quality is good
- ✅ DO add any missing tests if needed

---

## Phase 6: Create Pull Request

**Workflow Status:** TDD cycle complete, ready for PR

**Workflow Action:**
The workflow automatically attempts to create a PR when Phase 5 completes. If you reach this phase, the feature is complete!

**What the Workflow Does:**
1. Checks if PR already exists
2. Creates PR if needed with summary
3. Links to test results
4. Marks workflow as complete

**Your Role:**
You typically won't be called in this phase - the workflow handles PR creation. If you are called, it's because manual intervention is needed.

---

## How the Workflow-Driven TDD Loop Works

```
User creates issue with @claude
         ↓
   [Workflow Triggered]
         ↓
   ┌─────────────────┐
   │  Run Tests      │ ← Tests don't exist yet
   └────────┬────────┘
            ↓
   ┌─────────────────┐
   │ Determine Phase │ → Phase 2: Write Tests
   └────────┬────────┘
            ↓
   ┌─────────────────┐
   │  Claude Code    │ → Writes failing tests, commits, pushes
   └────────┬────────┘
            ↓
   ┌─────────────────┐
   │ Trigger Next    │ → Triggers workflow again
   └────────┬────────┘
            ↓
   [Workflow Triggered Again]
         ↓
   ┌─────────────────┐
   │  Run Tests      │ → All tests fail (expected!)
   └────────┬────────┘
            ↓
   ┌─────────────────┐
   │ Determine Phase │ → Phase 3: Implement
   └────────┬────────┘
            ↓
   ┌─────────────────┐
   │  Claude Code    │ → Implements code, commits, pushes
   └────────┬────────┘
            ↓
   ┌─────────────────┐
   │ Trigger Next    │ → Triggers workflow again
   └────────┬────────┘
            ↓
         [Loop continues until all tests pass]
            ↓
   ┌─────────────────┐
   │ Determine Phase │ → Phase 4: Refactor
   └────────┬────────┘
            ↓
   ┌─────────────────┐
   │  Claude Code    │ → Refactors, commits, pushes
   └────────┬────────┘
            ↓
   ┌─────────────────┐
   │  Run Tests      │ → All still pass
   └────────┬────────┘
            ↓
   ┌─────────────────┐
   │ Determine Phase │ → Phase 5: Final Verification
   └────────┬────────┘
            ↓
   ┌─────────────────┐
   │  Claude Code    │ → Verifies completeness
   └────────┬────────┘
            ↓
   ┌─────────────────┐
   │  Create PR      │ → PR automatically created
   └─────────────────┘
```

## Important Reminders for Claude

⚠️ **NEVER run tests yourself** - The workflow runs them between iterations
⚠️ **NEVER try to complete multiple phases** - Focus on current phase only
⚠️ **NEVER skip writing tests first** - Phase 2 comes before Phase 3
⚠️ **ALWAYS commit and push your changes** - This triggers the next iteration
⚠️ **ALWAYS use appropriate commit prefixes** - test:, feat:, refactor:
⚠️ **ALWAYS stay focused on the current phase** - The workflow will guide you

---

## Unity PlayMode Test Guidelines

**File Location:** `Assets/Tests/PlayMode/[FeatureName]Tests.cs`

**Required Attributes:**
```csharp
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections;

[TestFixture]
public class FeatureNameTests
{
    [UnityTest]
    public IEnumerator TestName()
    {
        // Test implementation
        yield return null;
    }
}
```

**Best Practices:**
- Use `[UnityTest]` for async/coroutine tests
- Use `[Test]` for synchronous tests
- Clean up test objects in `[TearDown]`
- Use `LogAssert` for testing Debug.Log calls
- Leverage `yield return new WaitForSeconds()` for timing tests

---

## How Workflow-Driven TDD Works

### Workflow Architecture

The TDD workflow consists of 5 jobs that run sequentially:

1. **run-tests**: Uses `game-ci/unity-test-runner` to run PlayMode tests
   - Skipped on initial issue creation (no tests yet)
   - Outputs test counts and results
   - Continues even if tests fail (expected in TDD!)

2. **determine-phase**: Analyzes test results to determine TDD phase
   - Checks if tests exist
   - Counts passing/failing tests
   - Examines last commit message
   - Outputs phase number and instructions

3. **claude-code**: Runs Claude with phase-specific context
   - Receives test results from previous run
   - Gets specific instructions for current phase
   - Makes changes and commits/pushes
   - Unity is pre-activated in the container

4. **trigger-next-iteration**: Automatically triggers next workflow run
   - Waits for changes to be pushed
   - Triggers workflow again via `workflow_dispatch`
   - Increments iteration counter
   - Stops after Phase 5

5. **create-pr**: Creates pull request when Phase 5 completes
   - Checks if PR already exists
   - Creates PR with summary if needed
   - Marks workflow as complete

### Phase Determination Logic

```
No tests exist → Phase 2 (Write Tests)
Tests exist, all fail → Phase 3 (Implement)
Tests exist, some fail → Phase 3 (Continue Implementing)
Tests exist, all pass, last commit not refactor → Phase 4 (Refactor)
Tests exist, all pass, last commit was refactor → Phase 5 (Verify)
Phase 5 complete → Create PR
```

### Environment

- **Container**: `unityci/editor:ubuntu-6000.0.31f1-base-3`
- **Unity License**: Pre-activated in container
- **Test Runner**: game-ci/unity-test-runner@v4
- **Available Environment Variables**:
  - `UNITY_LICENSE` - License file (configured)
  - `UNITY_EMAIL` - Account email (configured)
  - `UNITY_PASSWORD` - Account password (configured)
  - `TDD_PHASE` - Current phase number
  - `TDD_PHASE_NAME` - Human-readable phase name

### Key Insights

1. **The workflow IS the TDD loop** - Tests → Phase → Code → Tests → ...
2. **Claude never runs tests** - The workflow does it between iterations
3. **Each iteration is one phase** - Claude focuses on single phase only
4. **Automatic continuation** - Workflow loops until feature complete
5. **Test-driven phase detection** - Test state determines what to do next
