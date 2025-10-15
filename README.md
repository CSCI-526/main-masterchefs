# Order Up! — Unity WebGL Project

**Order-Up!** is a Unity-based cooking puzzle game currently under development.  
The project uses **GitHub Actions** for automatic WebGL builds and deployment to **GitHub Pages**, following a structured branching workflow for stable releases and team collaboration.

Key:
- Bread = pink oval
- Cheese = orange triangle
- Chicken = pink hexagon
- Dough = tan circle
- Egg = black oval
- Lettuce = green circle
- Pasta = small tan diamond
- Potato = brown oval
- Tomato = red circle

---

## 🧠 Repository Structure
```
Order-Up/
├── Assets/
├── Packages/
├── ProjectSettings/
├── .github/
│ └── workflows/
│  └── deploy.yml # GitHub Actions: Build + Deploy to GitHub Pages
└── README.md
```
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
```
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
```

---

## 🧩 Developer Guidelines

### **1️⃣ Create a New Feature Branch**
Whenever starting a new feature or bug fix:
```bash
git checkout dev
git pull origin dev
git checkout -b feature/feature-name
```

### **2️⃣ Commit and Push Changes**
Once your work is done:
```bash
git add .
git commit -m "Add new feature: [feature-name]"
git push origin feature/feature-name
```

### **3️⃣ Open a Pull Request to `dev`**
Submit a PR for review:
- PR **from:** `feature/<your-feature-name>`  
- PR **into:** `dev`  
- Add a clear description and assign a reviewer.

### **4️⃣ Review & Merge**
- Reviewer verifies the change and approves.
- Merge the PR into `dev`.

### **5️⃣ Promote to `main` & Deploy**
- When `dev` is stable, open a PR `dev` → `main`.
- Merge into `main` triggers the GitHub Action to build and deploy to `gh-pages`.

✅ **No manual folder deletion or switching branches required!**

---

## 🧰 Git Commands Reference

```bash
# Switch branches
git checkout <branch>

# Pull latest changes
git pull origin <branch>

# Create a new feature branch
git checkout -b feature/<feature-name>

# Push new branch to remote
git push origin feature/<feature-name>

# Merge dev into your branch to stay updated
git merge dev

# Delete a local branch after merge
git branch -d feature/<feature-name>
```
---
## 🧑‍💻 Contributors

- [Add teammates here] — Roles (Designer / Artist / QA / etc.)

---

## 📄 License

This project is licensed under the [MIT License](LICENSE).

---

**Notes & Tips**
- Keep feature branches small and focused.
- Run local tests for major changes before merging to `dev`.
