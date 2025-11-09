# MORENGY - The Spirit of the North

A professional, mobile-first Next.js website celebrating the traditional Malagasy combat game from northern Madagascar.

## 🎮 About the Project

MORENGY is a fighting game and cultural preservation initiative that honors the martial arts heritage of northern Madagascar. This website serves as the central hub for game information, fighter profiles, cultural education, and community engagement.

## ✨ Features

- **Responsive Design**: Mobile-first approach with full tablet and desktop support
- **Cultural Authenticity**: All content vetted for accuracy and respect to Malagasy traditions
- **Dynamic Fighter Profiles**: Interactive cards with detailed biographies and stats
- **Interactive Timeline**: Visual history of Morengy from ancient origins to modern day
- **News & Events**: Blog-style updates on development and cultural topics
- **Media Gallery**: Lightbox-enabled gallery for arenas, fighters, and cultural content
- **Contact Forms**: Functional forms for community engagement
- **SEO Optimized**: Comprehensive meta tags, Open Graph, and structured data
- **Smooth Animations**: Framer Motion animations throughout
- **Accessibility**: ARIA roles, semantic HTML, keyboard navigation

## 🛠️ Tech Stack

- **Framework**: Next.js 15 (App Router)
- **Language**: TypeScript
- **Styling**: Tailwind CSS with custom Malagasy color palette
- **Animations**: Framer Motion
- **Icons**: Lucide React
- **Fonts**: Montserrat (headings), Poppins (body)

## 🎨 Color Palette

Inspired by the Malagasy flag:

- **Red**: #C0392B (primary actions, highlights)
- **Green**: #27AE60 (secondary actions, accents)
- **White**: #F9F9F9 (text, backgrounds)
- **Black**: #1A1A1A (backgrounds, text)
- **Dark Background**: #0D0D0D (main background)

## 📁 Project Structure

```
website/
├── app/                    # Next.js app directory
│   ├── about/             # About page
│   ├── contact/           # Contact page
│   ├── fighters/          # Fighters page with filters
│   ├── gallery/           # Gallery with lightbox
│   ├── game/              # Game info page
│   ├── news/              # News & events page
│   ├── layout.tsx         # Root layout with metadata
│   ├── page.tsx           # Home page
│   └── globals.css        # Global styles
├── components/            # Reusable components
│   ├── Navbar.tsx
│   ├── Footer.tsx
│   ├── HeroSection.tsx
│   ├── FighterCard.tsx
│   ├── Timeline.tsx
│   ├── NewsCard.tsx
│   └── SectionHeading.tsx
├── data/                  # Static data files
│   ├── fighters.ts        # Fighter profiles
│   ├── news.ts           # News posts
│   ├── timeline.ts       # Historical timeline
│   └── gallery.ts        # Gallery items
├── types/                 # TypeScript type definitions
│   └── index.ts
└── public/               # Static assets
    └── gallery/          # Gallery images
```

## 🚀 Getting Started

### Prerequisites

- Node.js 18+ and npm

### Installation

1. Navigate to the website directory:
   ```bash
   cd website
   ```

2. Install dependencies:
   ```bash
   npm install
   ```

3. Run the development server:
   ```bash
   npm run dev
   ```

4. Open [http://localhost:3000](http://localhost:3000) in your browser

## 📦 Build for Production

```bash
npm run build
npm start
```

## 🌐 Deployment

This project is optimized for deployment on Vercel:

1. Push to GitHub
2. Import repository in Vercel
3. Deploy with default settings

## 🎯 Pages Overview

### Home (`/`)
- Hero section with dynamic background
- Introduction to Morengy
- Featured fighters
- Latest news
- Call-to-action sections

### About (`/about`)
- Comprehensive Morengy history
- Cultural significance
- Interactive timeline
- Regional fighting styles
- Philosophy section

### Fighters (`/fighters`)
- Full fighter roster
- Filter by era and region
- Interactive flip cards
- Detailed stats and biographies

### Game (`/game`)
- Gameplay mechanics
- Game modes
- Arena information
- Media placeholders
- Beta sign-up CTA

### News (`/news`)
- Blog-style post listing
- Category filtering
- Newsletter subscription
- Upcoming events calendar

### Gallery (`/gallery`)
- Category-filtered media grid
- Lightbox viewer
- Navigation between items
- Content submission CTA

### Contact (`/contact`)
- Contact form
- Social media links
- Ways to get involved
- FAQ section

## 🔄 Customization

### Adding Fighters

Edit `data/fighters.ts` to add new fighter profiles with stats, moves, and biography.

### Adding News Posts

Edit `data/news.ts` to add new blog posts with categories and dates.

### Updating Timeline

Edit `data/timeline.ts` to add historical events.

### Styling

- Modify `tailwind.config.ts` for color palette changes
- Edit `app/globals.css` for global style adjustments

## ♿ Accessibility

- Semantic HTML throughout
- ARIA labels on interactive elements
- Keyboard navigation support
- High contrast text
- Focus indicators

## 📱 Responsive Breakpoints

- Mobile: < 768px
- Tablet: 768px - 1024px
- Desktop: > 1024px

## 🧪 Testing Checklist

- [ ] All pages load correctly
- [ ] Navigation works on all screen sizes
- [ ] Forms submit properly
- [ ] Filters function correctly
- [ ] Animations perform smoothly
- [ ] Mobile menu works
- [ ] Keyboard navigation
- [ ] Screen reader compatibility

## 📄 License

This project celebrates Malagasy culture and is created with respect for traditional Morengy practitioners and cultural heritage.

## 🤝 Contributing

Contributions welcome! Please ensure:
- Cultural accuracy and sensitivity
- Code quality and documentation
- Mobile-first responsive design
- Accessibility standards

## 📞 Contact

For questions or collaboration: contact@morengy.com

---

**🎭 Built with respect for Malagasy culture and heritage**
