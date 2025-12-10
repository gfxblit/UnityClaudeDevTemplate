# Test-Driven Development Workflow for Unity PlayMode Tests

## Overview
This workflow enforces strict Test-Driven Development (TDD) for Unity features using PlayMode tests. Follow each phase sequentially.

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

**Process:**
1. Create or locate PlayMode test file in `Assets/Tests/PlayMode/`
2. Write tests that verify ALL requirements:
   - One test per acceptance criterion
   - Use descriptive test names: `When[Condition]_Should[ExpectedBehavior]`
   - Test edge cases and error conditions
   - Tests MUST fail initially (no implementation exists yet)
3. Run tests to confirm they fail
4. Verify failure messages are clear and helpful

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

**Exit Criteria:**
- All tests written and documented
- All tests fail with clear error messages
- Test run output confirmed

---

## Phase 3: Implement Minimum Code

**Process:**
1. Write the **simplest possible code** to make ONE test pass
2. No premature optimization or extra features
3. Hard-code values if necessary (will refactor later)
4. Run tests after each change
5. If test passes, commit that change
6. Move to next failing test
7. Repeat until ALL tests pass

**Rules:**
- ❌ Do NOT add features not required by tests
- ❌ Do NOT refactor yet (that's Phase 4)
- ✅ DO use the simplest solution
- ✅ DO make incremental changes
- ✅ DO verify each test individually

**Exit Criteria:** ALL tests pass (green)

---

## Phase 4: Refactor Implementation

**Process:**
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
3. After EACH refactor:
   - Run all tests
   - Ensure all tests still pass
   - If tests fail, revert the refactor
4. Commit each successful refactor separately

**Exit Criteria:**
- Clean, modular implementation
- All tests still passing
- Code follows Unity best practices

---

## Phase 5: Final Verification

**Process:**
1. Run complete test suite one final time
2. Verify all tests pass
3. Review test coverage:
   - All requirements tested?
   - Edge cases covered?
   - Error conditions handled?
4. If gaps found, return to Phase 2

**Exit Criteria:**
- 100% of requirements have passing tests
- No test failures
- Code is clean and refactored

---

## Phase 6: Create Pull Request

**Process:**
1. Commit all changes with descriptive messages:
   ```
   test: Add PlayMode tests for [feature]
   feat: Implement [feature] with TDD approach
   refactor: Simplify [component] logic
   ```
2. Push to feature branch
3. Create PR with:
   - **Title:** Concise feature description
   - **Summary:** List of implemented requirements
   - **Test Plan:**
     - List all test cases
     - Confirm all tests passing
     - Note any edge cases covered

**Template:**
```markdown
## Summary
Implemented [feature] using TDD approach

## Requirements Met
- [ ] Requirement 1
- [ ] Requirement 2

## Test Coverage
- All requirements covered by PlayMode tests
- [X] tests passing
- Edge cases tested: [list]

## Implementation Notes
[Brief description of approach]
```

**Exit Criteria:** PR created with complete documentation

---

## TDD Workflow Checklist

Use this checklist for each feature:

- [ ] Phase 1: Requirements clarified and documented
- [ ] Phase 2: Failing tests written (all requirements covered)
- [ ] Phase 3: Minimal implementation (all tests green)
- [ ] Phase 4: Refactored to clean, modular code
- [ ] Phase 5: Final verification passed
- [ ] Phase 6: PR created with documentation

---

## Important Reminders

⚠️ **NEVER skip writing tests first**
⚠️ **NEVER implement without failing tests**
⚠️ **NEVER refactor before tests pass**
⚠️ **NEVER commit failing tests**
⚠️ **ALWAYS run tests after changes**
⚠️ **ALWAYS clarify unclear requirements before coding**

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
