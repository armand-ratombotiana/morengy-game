"use client";

import { useState } from "react";
import type { Metadata } from "next";
import { motion } from "framer-motion";
import { HeroSection } from "@/components/HeroSection";
import { SectionHeading } from "@/components/SectionHeading";
import { NewsCard } from "@/components/NewsCard";
import { newsPosts } from "@/data/news";
import { Filter } from "lucide-react";
import type { NewsPost } from "@/types";

export default function NewsPage() {
  const [selectedCategory, setSelectedCategory] = useState<
    "all" | NewsPost["category"]
  >("all");

  const filteredPosts =
    selectedCategory === "all"
      ? newsPosts
      : newsPosts.filter((post) => post.category === selectedCategory);

  return (
    <div className="min-h-screen">
      {/* Hero Section */}
      <HeroSection
        title="News & Events"
        subtitle="Stay updated on game development, fighters, and Malagasy culture"
      />

      {/* News Section */}
      <section className="py-20 bg-morengy-dark-bg">
        <div className="container mx-auto px-4">
          <SectionHeading
            title="Latest Updates"
            subtitle="Follow our journey to bring Morengy to the world"
          />

          {/* Category Filter */}
          <motion.div
            className="max-w-4xl mx-auto mb-12"
            initial={{ opacity: 0, y: 20 }}
            animate={{ opacity: 1, y: 0 }}
          >
            <div className="bg-morengy-black border border-morengy-red/30 rounded-lg p-6">
              <div className="flex items-center gap-2 mb-4">
                <Filter size={20} className="text-morengy-red" />
                <h3 className="text-lg font-montserrat font-bold text-morengy-white">
                  Filter by Category
                </h3>
              </div>

              <div className="flex flex-wrap gap-3">
                <CategoryButton
                  active={selectedCategory === "all"}
                  onClick={() => setSelectedCategory("all")}
                >
                  All Posts
                </CategoryButton>
                <CategoryButton
                  active={selectedCategory === "Game Development"}
                  onClick={() => setSelectedCategory("Game Development")}
                >
                  Game Development
                </CategoryButton>
                <CategoryButton
                  active={selectedCategory === "Fighters"}
                  onClick={() => setSelectedCategory("Fighters")}
                >
                  Fighters
                </CategoryButton>
                <CategoryButton
                  active={selectedCategory === "Cultural Heritage"}
                  onClick={() => setSelectedCategory("Cultural Heritage")}
                >
                  Cultural Heritage
                </CategoryButton>
              </div>

              <div className="mt-4 text-sm text-morengy-white/60">
                Showing {filteredPosts.length} post
                {filteredPosts.length !== 1 ? "s" : ""}
              </div>
            </div>
          </motion.div>

          {/* News Grid */}
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-8 max-w-7xl mx-auto">
            {filteredPosts.map((post) => (
              <NewsCard key={post.id} post={post} />
            ))}
          </div>

          {/* No Results Message */}
          {filteredPosts.length === 0 && (
            <motion.div
              className="text-center py-12"
              initial={{ opacity: 0 }}
              animate={{ opacity: 1 }}
            >
              <p className="text-morengy-white/60 text-lg">
                No posts found in this category.
              </p>
            </motion.div>
          )}
        </div>
      </section>

      {/* Newsletter Section */}
      <section className="py-20 bg-morengy-black">
        <div className="container mx-auto px-4">
          <div className="max-w-3xl mx-auto">
            <SectionHeading
              title="Subscribe to Our Newsletter"
              subtitle="Get exclusive updates, behind-the-scenes content, and early access announcements"
            />

            <motion.div
              className="bg-gradient-to-br from-morengy-red-dark to-morengy-green-dark p-8 rounded-lg"
              initial={{ opacity: 0, y: 20 }}
              whileInView={{ opacity: 1, y: 0 }}
              viewport={{ once: true }}
            >
              <form className="space-y-4" id="newsletter">
                <div>
                  <label
                    htmlFor="email"
                    className="block text-sm text-morengy-white/90 mb-2"
                  >
                    Email Address
                  </label>
                  <input
                    type="email"
                    id="email"
                    name="email"
                    placeholder="your.email@example.com"
                    className="w-full bg-morengy-white/10 border border-morengy-white/30 rounded-lg px-4 py-3 text-morengy-white placeholder-morengy-white/50 focus:outline-none focus:border-morengy-white transition-colors"
                    required
                  />
                </div>

                <div className="flex items-start gap-3">
                  <input
                    type="checkbox"
                    id="consent"
                    name="consent"
                    className="mt-1"
                    required
                  />
                  <label
                    htmlFor="consent"
                    className="text-sm text-morengy-white/80"
                  >
                    I agree to receive updates about MORENGY and understand I
                    can unsubscribe at any time.
                  </label>
                </div>

                <button
                  type="submit"
                  className="w-full bg-morengy-white hover:bg-morengy-white/90 text-morengy-black font-montserrat font-bold py-3 rounded-lg transition-all duration-300 transform hover:scale-105"
                >
                  Subscribe Now
                </button>
              </form>
            </motion.div>
          </div>
        </div>
      </section>

      {/* Upcoming Events Section */}
      <section className="py-20 bg-morengy-dark-bg">
        <div className="container mx-auto px-4">
          <SectionHeading
            title="Upcoming Events"
            subtitle="Mark your calendar for these important dates"
          />

          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6 max-w-6xl mx-auto">
            <EventCard
              date="Q1 2025"
              title="Mobile Demo Release"
              description="Limited mobile demo featuring 3 fighters and 2 arenas available for beta testers."
            />
            <EventCard
              date="Q2 2025"
              title="Community Tournament"
              description="Online tournament open to all beta testers with exclusive prizes and fighter skins."
            />
            <EventCard
              date="Q3 2025"
              title="Full Roster Reveal"
              description="Complete fighter lineup announcement with detailed biographies and gameplay trailers."
            />
            <EventCard
              date="Q4 2025"
              title="Early Access Launch"
              description="Early access version available on select platforms with expanded content."
            />
            <EventCard
              date="2026"
              title="Official Release"
              description="Full game release with story mode, online ranked play, and all features."
            />
            <EventCard
              date="TBA"
              title="Madagascar Cultural Festival"
              description="Live event celebrating Malagasy culture with real Morengy demonstrations."
            />
          </div>
        </div>
      </section>

      {/* Social Media CTA */}
      <section className="py-20 bg-gradient-to-br from-morengy-red-dark via-morengy-black to-morengy-green-dark">
        <div className="container mx-auto px-4">
          <div className="max-w-3xl mx-auto text-center">
            <h2 className="text-3xl md:text-4xl font-montserrat font-black text-morengy-white mb-6">
              Follow Our Journey
            </h2>
            <p className="text-lg text-morengy-white/80 mb-8">
              Join our growing community on social media for daily updates,
              exclusive content, and behind-the-scenes development insights.
            </p>
            <div className="flex flex-wrap justify-center gap-4">
              <SocialButton href="#" platform="Instagram" />
              <SocialButton href="#" platform="YouTube" />
              <SocialButton href="#" platform="Twitter" />
              <SocialButton href="#" platform="Discord" />
            </div>
          </div>
        </div>
      </section>
    </div>
  );
}

interface CategoryButtonProps {
  active: boolean;
  onClick: () => void;
  children: React.ReactNode;
}

function CategoryButton({ active, onClick, children }: CategoryButtonProps) {
  return (
    <button
      onClick={onClick}
      className={`px-4 py-2 rounded-lg font-semibold transition-all duration-300 ${
        active
          ? "bg-morengy-red text-white"
          : "bg-morengy-white/10 text-morengy-white/70 hover:bg-morengy-white/20"
      }`}
    >
      {children}
    </button>
  );
}

interface EventCardProps {
  date: string;
  title: string;
  description: string;
}

function EventCard({ date, title, description }: EventCardProps) {
  return (
    <motion.div
      className="bg-morengy-black border border-morengy-green/30 rounded-lg p-6 hover:border-morengy-red/50 transition-colors duration-300"
      whileHover={{ y: -5 }}
      initial={{ opacity: 0, y: 20 }}
      whileInView={{ opacity: 1, y: 0 }}
      viewport={{ once: true }}
    >
      <div className="text-morengy-green font-montserrat font-bold text-sm mb-2">
        {date}
      </div>
      <h3 className="text-xl font-montserrat font-bold text-morengy-white mb-3">
        {title}
      </h3>
      <p className="text-morengy-white/70 text-sm leading-relaxed">
        {description}
      </p>
    </motion.div>
  );
}

interface SocialButtonProps {
  href: string;
  platform: string;
}

function SocialButton({ href, platform }: SocialButtonProps) {
  return (
    <a
      href={href}
      className="px-6 py-3 bg-morengy-white/10 hover:bg-morengy-white/20 border border-morengy-white/30 text-morengy-white font-semibold rounded-lg transition-all duration-300 transform hover:scale-105"
    >
      {platform}
    </a>
  );
}
