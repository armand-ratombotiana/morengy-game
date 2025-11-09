# MORENGY - Quick Start Guide

## 🚀 Get Started in 3 Steps

### 1️⃣ Navigate to Website Directory
```bash
cd website
```

### 2️⃣ Install Dependencies
```bash
npm install
```

### 3️⃣ Run Development Server
```bash
npm run dev
```

**Open your browser:** [http://localhost:3000](http://localhost:3000)

---

## 🎯 That's it! You're running MORENGY locally.

## 📚 What to Explore

- **Home** (`/`) - See the full hero section and overview
- **Fighters** (`/fighters`) - Browse and filter fighter profiles
- **About** (`/about`) - Learn Morengy history and view timeline
- **Game** (`/game`) - Explore gameplay mechanics and features
- **News** (`/news`) - Read blog posts and upcoming events
- **Gallery** (`/gallery`) - View media with lightbox
- **Contact** (`/contact`) - Test the contact form

## 🔧 Common Commands

```bash
# Development
npm run dev          # Start dev server (port 3000)

# Production
npm run build        # Build for production
npm start            # Run production build

# Quality
npm run lint         # Check code quality
```

## 📖 Need More Help?

- **Full Documentation**: See [website/README.md](website/README.md)
- **Deployment Guide**: See [website/DEPLOYMENT.md](website/DEPLOYMENT.md)
- **Project Summary**: See [PROJECT_SUMMARY.md](PROJECT_SUMMARY.md)

## 🌐 Ready to Deploy?

```bash
# Option 1: Vercel CLI
npm install -g vercel
cd website
vercel

# Option 2: Push to GitHub and import to Vercel
git add .
git commit -m "Ready to deploy"
git push
# Then import on vercel.com
```

## ✨ Features to Test

- ✅ Mobile menu (resize browser or use dev tools)
- ✅ Fighter cards (click to flip)
- ✅ Filter fighters by era/region
- ✅ Timeline animations (scroll down on About page)
- ✅ Gallery lightbox (click any gallery item)
- ✅ Contact form validation
- ✅ Smooth page transitions

## 🎨 Customization Tips

### Add Your Own Images
1. Place images in `website/public/gallery/`
2. Update paths in `website/data/gallery.ts`

### Add More Fighters
1. Edit `website/data/fighters.ts`
2. Follow the existing format

### Change Colors
1. Edit `website/tailwind.config.ts`
2. Modify the `morengy` color palette

### Add News Posts
1. Edit `website/data/news.ts`
2. Add new entries with category and date

## 🆘 Troubleshooting

**Port 3000 already in use?**
```bash
npx kill-port 3000
# or
npm run dev -- -p 3001
```

**Build errors?**
```bash
rm -rf .next node_modules
npm install
npm run build
```

**Need to reset?**
```bash
git reset --hard HEAD
npm install
```

## 📱 Test on Mobile

1. Find your local IP: `ipconfig` (Windows) or `ifconfig` (Mac/Linux)
2. Run `npm run dev`
3. On mobile browser: `http://YOUR_IP:3000`

## 🎉 You're All Set!

The MORENGY website is now running locally. Explore, customize, and deploy when ready!

**Questions?** Check the detailed documentation in the `website/` directory.

---

**🥊 Enjoy building with MORENGY - The Spirit of the North! 🥊**
