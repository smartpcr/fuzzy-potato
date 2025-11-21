# Prompt History

This directory contains a calendar-style journal of all interactions with Claude Code for this project.

## Structure

```
.claude/prompts/
├── {year}/
│   ├── {month}/
│   │   ├── {date}.md         # Daily prompt history
│   │   └── ...
│   └── ...
├── README.md                  # This file
└── template.md                # Template for new entries
```

## Format

Each daily log file follows this structure:

```markdown
# Prompt History - {Month Day, Year}

## Session {N}: {Brief Description}

### {Time} - {Topic}
**User:**
> {User's prompt}

**Response:**
{Summary of assistant's response}
{Key points}
{Decisions made}

## Technology Stack
{Technologies used in this session}

## Key Decisions
{Important architectural or design decisions}

## Notes
{Additional context, issues, or follow-ups}
```

## Usage

- Files are organized by date: `YYYY/MM/YYYY-MM-DD.md`
- Each session within a day is numbered sequentially
- Timestamps use 12-hour format with AM/PM
- Summaries capture the essence of responses, not verbatim transcripts
- Key decisions and technology choices are highlighted for future reference

## Purpose

The prompt history serves multiple purposes:

1. **Context Preservation**: Maintains conversation history across sessions
2. **Decision Log**: Documents why certain technical decisions were made
3. **Learning Record**: Captures problem-solving approaches and solutions
4. **Project Timeline**: Provides chronological development history
5. **Knowledge Transfer**: Helps onboard new team members or AI assistants

## Example Entry

See `2025/11/2025-11-20.md` for the project's initial setup session.
