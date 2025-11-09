# MORENGY Website - Deployment Guide

## 🚀 Quick Start (Local Development)

1. **Navigate to the website directory:**
   ```bash
   cd website
   ```

2. **Install dependencies:**
   ```bash
   npm install
   ```

3. **Run development server:**
   ```bash
   npm run dev
   ```

4. **Open in browser:**
   Navigate to [http://localhost:3000](http://localhost:3000)

## 📦 Available Scripts

- `npm run dev` - Start development server (port 3000)
- `npm run build` - Create production build
- `npm start` - Start production server
- `npm run lint` - Run ESLint checks

## 🌐 Deploy to Vercel (Recommended)

### Method 1: Vercel CLI

1. **Install Vercel CLI:**
   ```bash
   npm install -g vercel
   ```

2. **Login to Vercel:**
   ```bash
   vercel login
   ```

3. **Deploy from website directory:**
   ```bash
   cd website
   vercel
   ```

4. **Follow prompts:**
   - Set up and deploy: Yes
   - Which scope: Select your account
   - Link to existing project: No
   - Project name: morengy-website
   - Directory: ./
   - Override settings: No

5. **Deploy to production:**
   ```bash
   vercel --prod
   ```

### Method 2: Vercel Dashboard (GitHub Integration)

1. **Push to GitHub:**
   ```bash
   git add .
   git commit -m "Complete MORENGY website"
   git push origin main
   ```

2. **Import to Vercel:**
   - Go to [vercel.com](https://vercel.com)
   - Click "Add New Project"
   - Import your GitHub repository
   - Select the `website` directory as root
   - Click "Deploy"

3. **Configure Project:**
   - Framework Preset: Next.js
   - Root Directory: website
   - Build Command: `npm run build`
   - Output Directory: `.next`
   - Install Command: `npm install`

4. **Environment Variables (Optional):**
   Add any required environment variables in Vercel dashboard

## 🔧 Environment Variables

Currently, no environment variables are required. If you add features requiring environment variables (e.g., contact form backend, CMS integration), create a `.env.local` file:

```env
# Example environment variables
NEXT_PUBLIC_SITE_URL=https://morengy.com
CONTACT_FORM_API_KEY=your_api_key_here
```

## 📊 Performance Optimization

The website is already optimized with:

- ✅ Static page generation where possible
- ✅ Image optimization ready (add images to `/public`)
- ✅ CSS optimization with Tailwind
- ✅ Code splitting by route
- ✅ Efficient bundling

### Adding Images

1. Place images in `public/` directory:
   ```
   public/
   ├── gallery/
   │   ├── arena-antsiranana.jpg
   │   ├── fighter-randriana.jpg
   │   └── ...
   └── og-image.jpg (for social media)
   ```

2. Update image references in components to use actual paths

3. Use Next.js Image component for automatic optimization:
   ```tsx
   import Image from 'next/image';

   <Image
     src="/gallery/arena-antsiranana.jpg"
     alt="Antsiranana Arena"
     width={800}
     height={600}
   />
   ```

## 🔍 SEO Configuration

### Update Metadata Base URL

In `app/layout.tsx`, add metadataBase for production:

```tsx
export const metadata: Metadata = {
  metadataBase: new URL('https://your-domain.com'),
  // ... rest of metadata
};
```

### Add Sitemap

Create `app/sitemap.ts`:

```tsx
import { MetadataRoute } from 'next';

export default function sitemap(): MetadataRoute.Sitemap {
  return [
    {
      url: 'https://your-domain.com',
      lastModified: new Date(),
      changeFrequency: 'weekly',
      priority: 1,
    },
    {
      url: 'https://your-domain.com/about',
      lastModified: new Date(),
      changeFrequency: 'monthly',
      priority: 0.8,
    },
    // Add all pages...
  ];
}
```

### Add robots.txt

Create `app/robots.ts`:

```tsx
import { MetadataRoute } from 'next';

export default function robots(): MetadataRoute.Robots {
  return {
    rules: {
      userAgent: '*',
      allow: '/',
    },
    sitemap: 'https://your-domain.com/sitemap.xml',
  };
}
```

## 🎨 Custom Domain

### On Vercel:

1. Go to your project settings
2. Navigate to "Domains"
3. Add your custom domain
4. Configure DNS records as instructed
5. Wait for DNS propagation (can take up to 48 hours)

### DNS Configuration Example:

```
Type    Name    Value
A       @       76.76.21.21
CNAME   www     cname.vercel-dns.com
```

## 📱 Testing Checklist

Before deploying to production:

- [ ] Test all pages load correctly
- [ ] Check mobile responsiveness on multiple devices
- [ ] Verify all links work
- [ ] Test forms (contact, newsletter)
- [ ] Validate filters on Fighters and News pages
- [ ] Test gallery lightbox functionality
- [ ] Check animations perform smoothly
- [ ] Verify SEO meta tags
- [ ] Test social media sharing (Open Graph)
- [ ] Check accessibility with screen reader
- [ ] Validate keyboard navigation
- [ ] Test on different browsers (Chrome, Firefox, Safari, Edge)

## 🐛 Troubleshooting

### Build Fails

1. Clear `.next` folder and node_modules:
   ```bash
   rm -rf .next node_modules
   npm install
   npm run build
   ```

2. Check Node.js version (requires 18+):
   ```bash
   node --version
   ```

### Port Already in Use

```bash
# Kill process on port 3000
npx kill-port 3000

# Or use different port
npm run dev -- -p 3001
```

### TypeScript Errors

```bash
# Check types
npm run type-check

# If type-check script doesn't exist, add to package.json:
"type-check": "tsc --noEmit"
```

## 📈 Analytics Setup (Optional)

### Google Analytics

1. Add environment variable:
   ```env
   NEXT_PUBLIC_GA_ID=G-XXXXXXXXXX
   ```

2. Create `app/analytics.tsx` and add to layout

### Vercel Analytics

1. Install:
   ```bash
   npm install @vercel/analytics
   ```

2. Add to root layout:
   ```tsx
   import { Analytics } from '@vercel/analytics/react';

   export default function RootLayout({ children }) {
     return (
       <html>
         <body>
           {children}
           <Analytics />
         </body>
       </html>
     );
   }
   ```

## 🔒 Security Headers (Vercel)

Create `vercel.json` in project root:

```json
{
  "headers": [
    {
      "source": "/(.*)",
      "headers": [
        {
          "key": "X-Content-Type-Options",
          "value": "nosniff"
        },
        {
          "key": "X-Frame-Options",
          "value": "DENY"
        },
        {
          "key": "X-XSS-Protection",
          "value": "1; mode=block"
        }
      ]
    }
  ]
}
```

## 📞 Support

For deployment issues:
- Vercel Documentation: https://vercel.com/docs
- Next.js Documentation: https://nextjs.org/docs
- Project Issues: contact@morengy.com

## ✅ Post-Deployment

After successful deployment:

1. Test the live site thoroughly
2. Set up monitoring (Vercel Analytics, Sentry, etc.)
3. Configure custom domain
4. Submit sitemap to Google Search Console
5. Share on social media
6. Collect user feedback
7. Plan content updates and feature additions

---

**Deployed successfully? Congratulations! 🎉**

Your MORENGY website is now live and celebrating Malagasy culture with the world!
