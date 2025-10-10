# Order Up! — Unity WebGL Project

**Order-Up!** is a Unity-based cooking puzzle game currently under development.  
The project uses **GitHub Actions** for automatic WebGL builds and deployment to **GitHub Pages**, following a structured branching workflow for stable releases and team collaboration.

---

## 🧠 Repository Structure
Order-Up/
├── Assets/
├── Packages/
├── ProjectSettings/
├── .github/
│ └── workflows/
│  └── deploy.yml # GitHub Actions: Build + Deploy to GitHub Pages
└── README.md

---

## 🌿 Branch Workflow Overview

This repository uses a **three-branch model** for clean and scalable development.

| Branch | Purpose | Who works here |
|--------|----------|----------------|
| `main` | Stable, production-ready code. Automatically builds and deploys to GitHub Pages. | Maintainers only |
| `dev` | Integration and testing branch. Reviewed features are merged here before going to production. | All developers |
| `feature/<name>` | Temporary branches for developing individual features or fixes. | Individual contributors |

---

## 🔁 Branch Flow Diagram

Below is a visual representation of how code moves through the branches:
    ┌──────────────────────────────┐
    │     feature/<new-feature>    │
    │  (individual developer work) │
    └──────────────┬───────────────┘
                   │
                   ▼
          [ Pull Request → dev ]
                   │
                   ▼
          ┌───────────────────┐
          │       dev         │
          │  (testing branch) │
          └──────────┬────────┘
                     │
                     ▼
            [ Pull Request → main ]
                     │
                     ▼
          ┌───────────────────┐
          │       main        │
          │ (production build)│
          └──────────┬────────┘
                     │
                     ▼
             [ GitHub Action ]
                     │
                     ▼
          ┌───────────────────┐
          │     gh-pages      │
          │ (deployed site)   │
          └───────────────────┘
