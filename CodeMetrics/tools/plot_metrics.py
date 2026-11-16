#!/usr/bin/env python3
"""Regenerate the sprint summary and standalone SVG using only Python's standard library."""
import csv
import math
import re
from html import escape
from pathlib import Path
from statistics import mean

ROOT = Path(__file__).resolve().parents[1]
METRICS = [
    ("Maintainability Index", "Mean maintainability index", "Higher is better", "mean"),
    ("Cyclomatic Complexity", "Mean cyclomatic complexity", "Per type", "mean"),
    ("Class Coupling", "Mean class coupling", "Dependencies per type", "mean"),
    ("Depth of Inheritance", "Maximum inheritance depth", "Deepest type hierarchy", "max"),
    ("Lines of Source code", "Mean source lines", "Per type; includes comments and blank lines", "mean"),
    ("Lines of Executable code", "Mean executable lines", "Per type; analyzer statement estimate", "mean"),
]
SERIES = [("Game", "#087f8c"), ("MonoGameLibrary", "#8455b2")]


def collect():
    records = []
    files = sorted(ROOT.glob("CodeMetricsSprint*.csv"), key=lambda p: int(re.search(r"Sprint(\d+)", p.name)[1]))
    expected_header = None
    for path in files:
        sprint = int(re.search(r"Sprint(\d+)", path.name)[1])
        with path.open(newline="", encoding="utf-8-sig") as source:
            reader = csv.DictReader(source)
            if expected_header is None:
                expected_header = reader.fieldnames
            if reader.fieldnames != expected_header:
                raise ValueError(f"CSV header differs: {path}")
            rows = list(reader)
        for project, _ in SERIES:
            selected = [row for row in rows if (row["Project"] == "MonoGameLibrary") == (project == "MonoGameLibrary")]
            types = [row for row in selected if row["Scope"] == "Type"]
            members = [row for row in selected if row["Scope"] == "Member"]
            if not types:
                raise ValueError(f"No types for {project} in {path}")
            record = {"Sprint": sprint, "Project": project, "Type count": len(types), "Member row count": len(members)}
            for field, title, _, aggregation in METRICS:
                values = [int(row[field]) for row in types]
                record[title] = max(values) if aggregation == "max" else mean(values)
            records.append(record)
    return records


def number(value):
    return f"{value:.1f}" if value != int(value) else str(int(value))


def draw(records):
    sprints = sorted({record["Sprint"] for record in records})
    svg = ['<svg xmlns="http://www.w3.org/2000/svg" width="1280" height="1100" viewBox="0 0 1280 1100" role="img" aria-labelledby="title desc">',
           '<title id="title">Code metrics across sprints</title>',
           '<desc id="desc">Six charts compare type-level maintainability, complexity, coupling, inheritance, and code size for game and library code.</desc>',
           '<style>text{font-family:Arial,Helvetica,sans-serif;fill:#243247}.tick{font-size:12px;fill:#5c6b7d}.note{font-size:14px;fill:#5c6b7d}</style>',
           '<rect width="1280" height="1100" fill="#f4f7fb"/>',
           '<text x="42" y="48" font-size="28" font-weight="700">Code metrics across sprints</text>',
           '<text x="42" y="76" class="note">Type-level statistics · Game combines the historical sprint2 project and the current terraria project</text>']
    for index, (project, color) in enumerate(SERIES):
        x = 44 + index * 220
        svg += [f'<line x1="{x}" y1="105" x2="{x+25}" y2="105" stroke="{color}" stroke-width="3"/>',
                f'<text x="{x+34}" y="110" font-size="14">{escape(project)}</text>']
    for index, (field, title, subtitle, _) in enumerate(METRICS):
        left = 30 + (index % 2) * 625
        top = 133 + (index // 2) * 294
        x0, y0, width, height = left + 65, top + 88, 493, 150
        svg += [f'<rect x="{left}" y="{top}" width="600" height="278" rx="12" fill="white" stroke="#dce3ec"/>',
                f'<text x="{left+22}" y="{top+32}" font-size="18" font-weight="700">{escape(title)}</text>',
                f'<text x="{left+22}" y="{top+54}" class="note">{escape(subtitle)}</text>']
        peak = max(record[title] for record in records)
        maximum = 100 if field == "Maintainability Index" else max(1, math.ceil(peak * 1.2 / 4) * 4)
        for tick in range(5):
            value = maximum * tick / 4
            y = y0 + height * (1 - value / maximum)
            svg += [f'<line x1="{x0}" x2="{x0+width}" y1="{y}" y2="{y}" stroke="#e5eaf1"/>',
                    f'<text x="{x0-12}" y="{y+4}" text-anchor="end" class="tick">{number(value)}</text>']
        def x_for(sprint):
            return x0 + 26 + sprints.index(sprint) * (width - 52) / max(1, len(sprints) - 1)
        for sprint in sprints:
            svg.append(f'<text x="{x_for(sprint)}" y="{y0+height+24}" text-anchor="middle" class="tick">Sprint {sprint}</text>')
        for series_index, (project, color) in enumerate(SERIES):
            points = [(x_for(record["Sprint"]), y0 + height * (1 - record[title] / maximum), record[title])
                      for record in records if record["Project"] == project]
            svg.append(f'<polyline points="{" ".join(f"{x},{y}" for x,y,_ in points)}" fill="none" stroke="{color}" stroke-width="3"/>')
            for x, y, value in points:
                label_y = y - 12 if series_index == 0 else y + 19
                svg += [f'<circle cx="{x}" cy="{y}" r="4.5" fill="{color}" stroke="white" stroke-width="1.5"/>',
                        f'<text x="{x}" y="{label_y}" text-anchor="middle" font-size="12" font-weight="700" style="fill:{color}">{number(value)}</text>']
    counts = "; ".join(f'S{r["Sprint"]}: {r["Type count"]} game types' for r in records if r["Project"] == "Game")
    svg += [f'<text x="42" y="1044" class="note">{escape(counts)}. Averages change as features and types are added.</text>',
            '<text x="42" y="1068" class="note">Historical engine settings are unknown. Nested type source spans may overlap; these are averages, not unique file-line totals.</text>',
            '</svg>']
    (ROOT / "CodeMetricsTrends.svg").write_text("\n".join(svg) + "\n")


def main():
    records = collect()
    with (ROOT / "CodeMetricsSummary.csv").open("w", newline="") as output:
        writer = csv.DictWriter(output, fieldnames=list(records[0]))
        writer.writeheader()
        writer.writerows({key: round(value, 3) if isinstance(value, float) else value for key, value in record.items()} for record in records)
    draw(records)
    print("Generated CodeMetricsSummary.csv and CodeMetricsTrends.svg")


if __name__ == "__main__":
    main()
