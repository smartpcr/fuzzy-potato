# FuzzyPotato Knowledge Base Structure

This document visualizes the complete interconnected knowledge base for the FuzzyPotato project.

## Document Hierarchy

```
📁 fuzzy-potato/
├── 📄 README.md ─────────────────┐
│                                  │
├── 📁 .claude/                    │ Links to docs
│   │                              │
│   ├── 📁 agents/                 │
│   │   ├── 📄 index.md ◄──────────┘ Central hub
│   │   │       │
│   │   │       ├──► architecture.md    ◄──┐
│   │   │       │    (Build, CI/CD)        │
│   │   │       │         │                │
│   │   │       ├──► design.md            │ Cross-
│   │   │       │    (SOLID, Standards)    │ references
│   │   │       │         │                │
│   │   │       ├──► patterns.md          │
│   │   │       │    (Implementations) ◄───┤
│   │   │       │         │                │
│   │   │       ├──► test-strategy.md     │
│   │   │       │    (Testing approach) ◄──┤
│   │   │       │         │                │
│   │   │       └──► usage-guide.md       │
│   │   │            (Examples, IDEs) ◄────┘
│   │   │
│   │   └── 📁 All documents have:
│   │        ├── Top navigation bar
│   │        ├── Related documents section
│   │        └── Bottom navigation + key takeaways
│   │
│   └── 📁 prompts/
│       ├── README.md (Prompt history system)
│       ├── template.md (Entry template)
│       └── 📁 {year}/{month}/
│           └── {date}.md (Daily logs)
│
└── 📁 src/...
```

## Navigation Flow

### Primary Navigation Paths

```
1. Entry Point
   README.md → .claude/agents/index.md

2. Architecture Path
   index.md → architecture.md ↔ design.md ↔ patterns.md

3. Development Path
   index.md → usage-guide.md → test-strategy.md → patterns.md

4. Code Review Path
   patterns.md (EditorConfig check) → design.md → test-strategy.md

5. History Path
   index.md → .claude/prompts/README.md → daily logs
```

### Bidirectional Links

Every document links to related documents in both directions:

```
architecture.md ↔ design.md ↔ patterns.md
                    ↕            ↕
            test-strategy.md ↔ patterns.md
                    ↕
              usage-guide.md
                    ↕
                index.md
                    ↕
            prompts/README.md
```

## Cross-Reference Matrix

| Document       | Links To                                      | Linked From                           |
|----------------|-----------------------------------------------|---------------------------------------|
| index.md       | All documents, prompts/                       | README.md, all documents (bottom)     |
| architecture.md| design.md, patterns.md, test-strategy.md      | index.md, design.md, usage-guide.md   |
| design.md      | architecture.md, patterns.md, test-strategy.md| index.md, architecture.md, patterns.md|
| patterns.md    | design.md, architecture.md, test-strategy.md, usage-guide.md | index.md, design.md, test-strategy.md|
| test-strategy.md| patterns.md, design.md, architecture.md      | index.md, patterns.md, usage-guide.md |
| usage-guide.md | architecture.md, patterns.md, test-strategy.md, prompts/ | index.md, test-strategy.md       |
| prompts/README.md| template.md, daily logs                    | index.md, usage-guide.md              |

## Document Purpose & When to Use

### Architecture (architecture.md)
**When**: Need to understand project structure, build system, or CI/CD
**Key Topics**: Directory layout, MSBuild hierarchy, testing framework, versioning
**Links To**: design.md (for principles), patterns.md (for implementations)

### Design Principles (design.md)
**When**: Need to understand coding standards, SOLID principles, or async patterns
**Key Topics**: SRP, DIP, null handling, error handling, async/await
**Links To**: patterns.md (for concrete examples), architecture.md (for build config)

### Patterns (patterns.md)
**When**: Implementing features or reviewing code
**Key Topics**: ⚠️ **CRITICAL EditorConfig compliance**, Repository, Factory, Strategy patterns
**Links To**: design.md (for principles), test-strategy.md (for test patterns)

### Test Strategy (test-strategy.md)
**When**: Writing tests or checking coverage
**Key Topics**: AAA pattern, FluentAssertions, mocking, coverage targets
**Links To**: patterns.md (for test builders), design.md (for testing principles)

### Usage Guide (usage-guide.md)
**When**: Getting started or learning development workflows
**Key Topics**: Setup, build commands, IDE configuration, common tasks
**Links To**: All other docs as references, prompts for history

### Prompt History (prompts/)
**When**: Understanding past decisions or development timeline
**Key Topics**: Conversation logs, decision rationale, technology choices
**Linked From**: index.md, usage-guide.md

## Content Organization Principles

### 1. Hierarchical Structure
- **Level 1**: index.md (overview)
- **Level 2**: Core documents (architecture, design, patterns, tests, usage)
- **Level 3**: Prompts (historical context)

### 2. Bidirectional Navigation
Every document can reach every other document in ≤2 clicks:
- Direct link (1 click)
- Via index.md (2 clicks)

### 3. Context-Aware References
Documents link to others when:
- **Architecture** mentions build → links to design.md for standards
- **Design** mentions patterns → links to patterns.md for examples
- **Patterns** mentions testing → links to test-strategy.md
- **Tests** mention builders → links to patterns.md
- **Usage** references history → links to prompts/

### 4. Visual Navigation Aids
Each document has:
- Top bar: `[← Index] | [Doc1 →] | [Doc2 →]`
- Related docs section with descriptions
- Bottom bar with key takeaways + links

## Maintenance Guidelines

### When Adding New Documents
1. Add entry to index.md
2. Add cross-references from related docs
3. Add to cross-reference matrix (this file)
4. Update README.md if public-facing

### When Updating Content
1. Check if related docs need updates
2. Update prompt history with decision
3. Maintain bidirectional links
4. Keep navigation consistent

### When Making Architectural Changes
1. Update architecture.md (primary)
2. Update design.md (if principles affected)
3. Update patterns.md (if implementations change)
4. Update test-strategy.md (if testing changes)
5. Log in prompts/ with rationale

## Search Strategies

### For Architecture Decisions
1. Check prompts/ for historical context
2. Read architecture.md for current state
3. Check design.md for alignment with principles

### For Implementation Patterns
1. Start with patterns.md
2. Check design.md for underlying principles
3. See usage-guide.md for examples
4. Check test-strategy.md for testing approach

### For Code Review
1. **MUST**: Check patterns.md for EditorConfig compliance
2. Verify design.md principles adherence
3. Check test-strategy.md for coverage
4. Reference architecture.md for structure

## Integration Points

### With README.md
- README Development section links to index.md
- Provides quick overview of all documents
- Highlights critical documents (patterns.md EditorConfig)

### With Codebase
- patterns.md references actual code patterns
- architecture.md matches actual structure
- test-strategy.md aligns with test projects
- usage-guide.md uses real commands

### With Git History
- prompts/ provides narrative context
- Links decisions to specific dates
- Explains "why" behind technical choices
- Tracks technology stack evolution

## Benefits of This Structure

1. **Discoverability**: Every document is ≤2 clicks away
2. **Context**: Related docs linked when relevant
3. **History**: Prompt history preserves decisions
4. **Consistency**: Uniform navigation structure
5. **Scalability**: Easy to add new documents
6. **Maintainability**: Clear update guidelines
7. **Knowledge Transfer**: New developers can explore systematically

---

**Last Updated**: 2025-11-20
**Structure Version**: 1.0
**Documents**: 6 agent docs + prompt history system
