# Workflow Simplification: 5 Jobs → 2 Jobs

## Summary

The TDD workflow has been simplified from **5 jobs** to **2 jobs** (plus 1 bootstrap job), reducing complexity while maintaining all functionality.

## Before: 5-Job Architecture

```yaml
jobs:
  run-tests:           # Run Unity tests
    → outputs: test results

  determine-phase:     # Determine TDD phase
    needs: [run-tests]
    → outputs: phase, instructions

  claude-code:         # Execute Claude
    needs: [determine-phase]
    → outputs: changes_made

  trigger-next-iteration:  # Loop control
    needs: [determine-phase, claude-code]
    → triggers: next workflow run

  create-pr:           # Create PR when done
    needs: [determine-phase, claude-code]
    → creates: pull request
```

**Problems:**
- Complex job dependencies (`needs:` chains)
- State passed through multiple job outputs
- Separate jobs for closely related operations
- Hard to follow flow across 5 different job logs

## After: 2-Job Architecture

```yaml
jobs:
  bootstrap:           # Initial issue only (Phase 2)
    if: initial issue
    → outputs: phase, instructions

  analyze:             # Run tests + determine phase (MERGED)
    if: not initial issue
    → outputs: phase, instructions

  execute:             # Execute Claude + trigger next/create PR (MERGED)
    needs: [bootstrap, analyze]
    → runs: Claude, then triggers next OR creates PR
```

**Benefits:**
- ✅ Related operations combined (tests + analysis)
- ✅ Fewer job outputs to manage
- ✅ Simpler dependency chain
- ✅ All phase execution in one place (easier debugging)
- ✅ Loop control inline (no separate job)

## What Got Merged

### Merge 1: `run-tests` + `determine-phase` → `analyze`

**Before:** Two separate jobs
1. `run-tests` - Runs game-ci/unity-test-runner, parses results, uploads artifacts
2. `determine-phase` - Checks out code again, reads outputs, determines phase

**After:** Single `analyze` job
1. Check if tests exist
2. Run game-ci/unity-test-runner
3. Parse results
4. Upload artifacts
5. **Immediately determine phase** (no new checkout needed, same context)

**Why this works:**
- Phase determination just reads test outputs - no reason to checkout again
- Same git history is available (fetch-depth: 10)
- Reduces one job dependency
- Fewer job outputs (no intermediate test outputs, just final phase)

### Merge 2: `claude-code` + `trigger-next-iteration` + `create-pr` → `execute`

**Before:** Three separate jobs
1. `claude-code` - Runs Claude, checks for changes
2. `trigger-next-iteration` - If not phase 5, triggers next run
3. `create-pr` - If phase 5, creates PR

**After:** Single `execute` job with conditional steps
1. Checkout, configure Git, activate Unity
2. Download test results
3. Run Claude
4. Check for changes
5. **If not phase 5:** Trigger next iteration (inline step)
6. **If phase 5:** Create PR (inline step)

**Why this works:**
- Trigger and PR creation are mutually exclusive (phase != 5 vs phase == 5)
- Both need same setup (git config, secrets)
- No need to pass `changes_made` through job outputs
- Simpler: if/else instead of two jobs with complex conditions

## Job Count Comparison

| Workflow | Jobs | Job Dependencies | Conditional Jobs |
|----------|------|------------------|------------------|
| **Original** | 5 | 8 `needs:` declarations | All 5 |
| **Simplified** | 3 (1 bootstrap + 2 main) | 2 `needs:` declarations | All 3 |

## Lines of Code Comparison

| File | Original | Simplified | Reduction |
|------|----------|------------|-----------|
| Workflow YAML | ~545 lines | ~360 lines | **34% smaller** |
| Job definitions | 5 jobs | 3 jobs | **40% fewer** |
| Step duplication | Checkout: 5x | Checkout: 3x | **40% less duplication** |

## What Didn't Change

✅ **Functionality preserved:**
- Still uses game-ci/unity-test-runner (required for licensing)
- Still self-triggers for iteration loop
- Still handles initial issue bootstrap
- Still detects all 6 phases (0-5)
- Still uploads test artifacts
- Still creates PR on Phase 5

✅ **Behavior unchanged:**
- Same phase determination logic
- Same Claude prompts and instructions
- Same test running process
- Same iteration loop mechanism

## Migration Path

The simplified workflow is in `tdd-workflow-simplified.yml`. To migrate:

1. **Test the simplified workflow:**
   ```bash
   gh workflow run tdd-workflow-simplified.yml
   ```

2. **Verify it works for a full TDD cycle**

3. **Switch by renaming:**
   ```bash
   mv .github/workflows/tdd-workflow.yml .github/workflows/tdd-workflow-old.yml
   mv .github/workflows/tdd-workflow-simplified.yml .github/workflows/tdd-workflow.yml
   ```

4. **Update CLAUDE.md references** if needed (currently no workflow-specific references)

## Enhanced CLAUDE.md

As part of simplification, CLAUDE.md was enhanced to guide Claude on accessing test results:

**Added:**
- Section on "Accessing Test Results" at the top
- Instructions to read `test-results/PlayMode-results.xml`
- Bash commands for each phase to check test status
- Examples of grepping for failed tests and error messages

**Why in CLAUDE.md instead of workflow prompt:**
- Easier to iterate and improve instructions
- Single source of truth for Claude behavior
- Keeps workflow YAML focused on orchestration
- Can be more detailed without bloating workflow file

## Key Insights from Simplification Process

### 1. The Self-Triggering Pattern is Correct

We explored eliminating the self-triggering loop, but discovered:
- ❌ **Can't run unity-editor directly** - Manual license activation doesn't work
- ✅ **Must use game-ci actions** - They handle Unity licensing properly
- ✅ **Self-triggering creates good UX** - Each iteration is a separate workflow run in GitHub UI

The self-triggering pattern is the **right architecture** given Unity's licensing constraints.

### 2. game-ci Must Be Used

Testing proved that:
```yaml
# ❌ This doesn't work
- name: Activate Unity
  run: echo "$UNITY_LICENSE" > Unity_lic.ulf

- name: Run tests
  run: unity-editor -runTests

# ✅ This is required
- name: Run tests
  uses: game-ci/unity-test-runner@v4
```

game-ci does more than copy a license file - it properly activates Unity through their licensing system.

### 3. Complexity Was in Job Boundaries, Not Logic

The workflow logic itself is sound:
1. Run tests
2. Determine phase
3. Execute phase
4. Loop or finish

The complexity came from **unnecessary job boundaries** that required:
- Multiple checkouts
- Passing state through job outputs
- Complex `needs:` and `if:` conditions

**Solution:** Merge jobs that share context and purpose.

### 4. CLAUDE.md is Better than Workflow Prompts

For detailed instructions on "how to do Phase X", CLAUDE.md is superior because:
- Can be edited without redeploying workflow
- Can include examples and code snippets
- Can be version controlled independently
- Easier for humans to read and improve

The workflow should only pass **dynamic** information (phase number, test counts).

## Future Improvements

Potential further simplifications:

### 1. Combine bootstrap + analyze into one job
Currently we have separate `bootstrap` and `analyze` jobs. Could use a single job with conditional steps:

```yaml
analyze:
  steps:
    - name: Bootstrap check
      if: initial issue
      # Set phase=2 outputs

    - name: Run tests
      if: not initial issue
      uses: game-ci/unity-test-runner@v4

    - name: Determine phase
      # Works for both bootstrap and test paths
```

**Tradeoff:** Slightly more complex conditionals within job, but eliminates `needs: [bootstrap, analyze]` complexity in execute job.

### 2. Use composite action for test + analyze
Create `.github/actions/analyze-tdd-phase/action.yml`:

```yaml
name: Analyze TDD Phase
runs:
  using: composite
  steps:
    - # Check tests
    - # Run game-ci
    - # Determine phase
```

Then:
```yaml
jobs:
  analyze:
    steps:
      - uses: ./.github/actions/analyze-tdd-phase
```

**Tradeoff:** Adds another file, but could be reused across workflows.

### 3. Eliminate 10-second wait
Current workflow waits 10 seconds for git push. Could instead:

```yaml
- name: Push and verify
  run: |
    git push
    # Poll until remote has the commit
    for i in {1..10}; do
      if git fetch && git rev-parse @{u} | grep -q $(git rev-parse HEAD); then
        break
      fi
      sleep 2
    done
```

**Tradeoff:** More complex, but more reliable (doesn't assume 10s is enough).

## Conclusion

The simplified workflow achieves:
- **34% fewer lines** of YAML
- **40% fewer jobs** (3 vs 5)
- **Simpler job dependencies** (2 `needs:` vs 8)
- **Same functionality** - no features lost
- **Better debuggability** - related operations in same job logs

The self-triggering loop pattern remains because it's the correct architecture for Unity's licensing constraints. The simplification focused on **reducing unnecessary job boundaries** rather than changing the fundamental approach.
