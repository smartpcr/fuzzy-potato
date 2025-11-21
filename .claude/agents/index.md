# FuzzyPotato Context Documentation

Welcome to the FuzzyPotato project context documentation. This knowledge base helps Claude Code understand the project's architecture, design decisions, and development practices.

## 📚 Documentation Structure

### Folder Organization

```
.claude/
├── CLAUDE.md        # Instructions for maintaining this documentation system ⭐
├── agents/          # Context documentation for AI assistants
│   ├── index.md           ← You are here
│   ├── architecture.md
│   ├── design.md
│   ├── patterns.md
│   ├── test-strategy.md
│   └── usage-guide.md
└── prompts/         # Conversation history journal
    ├── README.md
    ├── template.md
    └── {year}/{month}/{date}.md
```

### Meta Documentation

**[.claude/CLAUDE.md](../CLAUDE.md)** - Instructions for Claude Code on maintaining this documentation system
- When to update prompt history
- How to structure session notes
- Documentation quality standards
- Cross-referencing guidelines
- **Read this first to understand the documentation workflow**

### Core Documents

1. **[Architecture](./architecture.md)** - System structure, build configuration, and CI/CD
   - Project organization
   - Build system hierarchy
   - Testing framework
   - CI/CD pipelines
   - Configuration management

2. **[Design Principles](./design.md)** - SOLID principles, coding standards, and best practices
   - SOLID principles in practice
   - Code organization strategies
   - Error handling patterns
   - Async/await guidelines
   - Dependency injection

3. **[Design Patterns](./patterns.md)** - Implementation patterns and anti-patterns
   - Common patterns (Repository, Factory, Strategy, etc.)
   - Functional programming patterns
   - Async patterns
   - Testing patterns
   - **CRITICAL**: EditorConfig compliance rules

### Specialized Guides

4. **[Test Strategy](./test-strategy.md)** - Testing approach and coverage requirements
   - Test organization
   - Unit test patterns
   - Integration test patterns
   - Code coverage targets
   - Test data builders

5. **[Usage Guide](./usage-guide.md)** - API usage examples and common scenarios
   - Quick start examples
   - Polymorphic serialization
   - Custom type creation
   - Advanced scenarios
   - Workflow serialization

### Prompt History

6. **[Prompt History](../prompts/README.md)** - Chronological development journal
   - Daily conversation logs
   - Decision timeline
   - Technology choices
   - Problem-solving approaches
   - See: [Latest prompts](../prompts/2025/11/)

## 🔗 Cross-Reference Map

```
                    ┌──────────────────────┐
                    │  .claude/CLAUDE.md   │ ◄─── Meta documentation
                    │  (Documentation)     │      (how to maintain docs)
                    │  (maintenance guide) │
                    └──────────┬───────────┘
                               │
                    ┌──────────▼───────────┐
                    │     index.md         │ ◄─── You are here
                    └──────────┬───────────┘
                               │
            ┌──────────────────┼──────────────────┬─────────────┬──────────────┐
            │                  │                  │             │              │
      ┌─────▼─────┐      ┌────▼──────┐     ┌─────▼────────┐ ┌─▼──────────┐ ┌─▼──────────┐
      │ arch-     │      │ design    │     │ patterns     │ │ test-      │ │ usage-     │
      │ itect-    │      │ .md       │     │ .md          │ │ strategy   │ │ guide.md   │
      │ ure.md    │      └───────────┘     └──────────────┘ │ .md        │ └────────────┘
      └───────────┘            │                  │          └────────────┘        │
                               │                  │                │               │
                               └──────────────────┴────────────────┴───────────────┘
                                       References each other
```

## 🎯 When to Use Each Document

### For Maintaining Documentation
1. **START HERE**: Read [.claude/CLAUDE.md](../CLAUDE.md) to understand the documentation system
2. Follow the guidelines for updating prompt history
3. Use the templates provided for consistency
4. Maintain cross-references between documents

### For Architecture Changes
1. Start with [Architecture](./architecture.md) to understand current structure
2. Reference [Design Principles](./design.md) for alignment
3. Check [Patterns](./patterns.md) for implementation approaches
4. Update [Test Strategy](./test-strategy.md) if testing needs change

### For Feature Development
1. Review [Design Principles](./design.md) for coding standards
2. Check [Patterns](./patterns.md) for applicable patterns
3. Follow [Test Strategy](./test-strategy.md) for test coverage
4. Update [Usage Guide](./usage-guide.md) with examples

### For Code Review
1. Verify against [Patterns](./patterns.md) - **MUST** check EditorConfig compliance
2. Validate [Design Principles](./design.md) adherence
3. Ensure [Test Strategy](./test-strategy.md) requirements met

### For Onboarding
1. Start with [Usage Guide](./usage-guide.md) for quick overview
2. Read [Architecture](./architecture.md) for project structure
3. Learn [Design Principles](./design.md) and [Patterns](./patterns.md)
4. Understand [Test Strategy](./test-strategy.md)

## 🔄 Document Update Guidelines

When making changes:

1. **Architecture changes** → Update:
   - architecture.md (primary)
   - design.md (if principles affected)
   - test-strategy.md (if testing framework changes)

2. **New patterns** → Update:
   - patterns.md (primary)
   - design.md (if new principles introduced)
   - usage-guide.md (add examples)

3. **New features** → Update:
   - usage-guide.md (primary)
   - test-strategy.md (test examples)
   - patterns.md (if new patterns used)

## 📋 Quick Reference Links

- **Documentation System**: See [.claude/CLAUDE.md](../CLAUDE.md) - **Start here for maintaining docs**
- **Build & Run**: See [Architecture - Build System](./architecture.md#build-system)
- **Code Style**: See [Patterns - EditorConfig Compliance](./patterns.md#-critical-editorconfig-compliance)
- **Testing**: See [Test Strategy](./test-strategy.md)
- **Examples**: See [Usage Guide](./usage-guide.md)
- **SOLID Principles**: See [Design Principles](./design.md#solid-principles)
- **Development History**: See [Prompt History](../prompts/)

## 🚀 Project Status

- **Current Phase**: Production-ready polymorphic serialization library
- **Latest Achievements** (Session 4):
  - ✅ Custom converter architecture for robust polymorphic serialization
  - ✅ TypeRegistry-based runtime type resolution
  - ✅ Nested polymorphic collections fully supported
  - ✅ All 24 tests passing (JSON + YAML)
  - ✅ Works across assembly boundaries
  - ✅ Comprehensive documentation system established
- **Key Files**:
  - `PolymorphicJsonConverter.cs` - Custom JSON converter
  - `PolymorphicYamlTypeInspector.cs` - Custom YAML deserializer
  - `.claude/CLAUDE.md` - Documentation maintenance guide
- **Status**: 🎉 **Production Ready**

---

**Note**: This documentation is maintained as context for Claude Code. Keep it up-to-date as the project evolves.
