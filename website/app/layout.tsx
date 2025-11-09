import type { Metadata } from "next";
import "./globals.css";
import { Navbar } from "@/components/Navbar";
import { Footer } from "@/components/Footer";

export const metadata: Metadata = {
  title: {
    default: "MORENGY - The Spirit of the North",
    template: "%s | MORENGY",
  },
  description:
    "Discover MORENGY, the traditional Malagasy combat game from northern Madagascar. Experience the fighting spirit, cultural heritage, and legendary warriors of Madagascar.",
  keywords: [
    "MORENGY",
    "Malagasy combat",
    "Madagascar fighting",
    "traditional martial arts",
    "northern Madagascar",
    "Malagasy culture",
    "fighting game",
    "cultural heritage",
  ],
  authors: [{ name: "MORENGY Team" }],
  creator: "MORENGY Team",
  openGraph: {
    type: "website",
    locale: "en_US",
    url: "https://morengy.com",
    title: "MORENGY - The Spirit of the North",
    description:
      "Discover MORENGY, the traditional Malagasy combat game from northern Madagascar.",
    siteName: "MORENGY",
    images: [
      {
        url: "/og-image.jpg",
        width: 1200,
        height: 630,
        alt: "MORENGY - The Spirit of the North",
      },
    ],
  },
  twitter: {
    card: "summary_large_image",
    title: "MORENGY - The Spirit of the North",
    description:
      "Discover MORENGY, the traditional Malagasy combat game from northern Madagascar.",
    images: ["/og-image.jpg"],
  },
  robots: {
    index: true,
    follow: true,
    googleBot: {
      index: true,
      follow: true,
      "max-video-preview": -1,
      "max-image-preview": "large",
      "max-snippet": -1,
    },
  },
};

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html lang="en" className="scroll-smooth">
      <body className="flex flex-col min-h-screen">
        <Navbar />
        <main className="flex-grow">{children}</main>
        <Footer />
      </body>
    </html>
  );
}
