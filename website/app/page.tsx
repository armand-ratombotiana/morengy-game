"use client";

import { motion } from "framer-motion";
import { HeroSection } from "@/components/HeroSection";
import { SectionHeading } from "@/components/SectionHeading";
import { FighterCard } from "@/components/FighterCard";
import { NewsCard } from "@/components/NewsCard";
import { fighters } from "@/data/fighters";
import { newsPosts } from "@/data/news";
import { Swords, Globe, Users, Gamepad2 } from "lucide-react";

export default function HomePage() {
  const featuredFighters = fighters.slice(0, 3);
  const latestNews = newsPosts.slice(0, 3);

  return (
    <div className="min-h-screen">
      {/* Hero Section */}
      <HeroSection
        title="The Malagasy Fighting Spirit"
        subtitle="Experience the ancient combat tradition of northern Madagascar in an epic fighting game that honors cultural heritage"
        primaryCTA={{ text: "Discover Morengy", href: "/about" }}
        secondaryCTA={{ text: "Join the Project", href: "/contact" }}
        showVideo={true}
      />

      {/* Introduction Section */}
      <section className="py-20 bg-morengy-dark-bg">
        <div className="container mx-auto px-4">
          <SectionHeading
            title="Welcome to MORENGY"
            subtitle="Where tradition meets innovation"
          />

          <div className="max-w-4xl mx-auto text-center mb-16">
            <motion.p
              className="text-lg text-morengy-white/80 leading-relaxed"
              initial={{ opacity: 0, y: 20 }}
              whileInView={{ opacity: 1, y: 0 }}
              viewport={{ once: true }}
            >
              MORENGY is more than a fighting game—it&apos;s a celebration of
              Malagasy culture and the warrior spirit of northern Madagascar.
              This traditional combat sport has been passed down through
              generations, combining physical prowess, strategic thinking, and
              deep cultural significance. Now, we&apos;re bringing this rich heritage
              to the world through interactive entertainment.
            </motion.p>
          </div>

          {/* Feature Cards */}
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mt-12">
            <FeatureCard
              icon={<Swords size={40} />}
              title="Authentic Combat"
              description="Experience fighting styles based on real Morengy techniques and traditions"
            />
            <FeatureCard
              icon={<Globe size={40} />}
              title="Cultural Heritage"
              description="Immerse yourself in the rich history and customs of northern Madagascar"
            />
            <FeatureCard
              icon={<Users size={40} />}
              title="Legendary Fighters"
              description="Play as historical masters and modern champions, each with unique styles"
            />
            <FeatureCard
              icon={<Gamepad2 size={40} />}
              title="Epic Battles"
              description="Engage in dynamic fights across authentic Malagasy arenas and landscapes"
            />
          </div>
        </div>
      </section>

      {/* Featured Fighters Section */}
      <section className="py-20 bg-morengy-black">
        <div className="container mx-auto px-4">
          <SectionHeading
            title="Featured Fighters"
            subtitle="Meet the warriors who define the spirit of Morengy"
          />

          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-8 max-w-7xl mx-auto">
            {featuredFighters.map((fighter, index) => (
              <motion.div
                key={fighter.id}
                initial={{ opacity: 0, y: 20 }}
                whileInView={{ opacity: 1, y: 0 }}
                viewport={{ once: true }}
                transition={{ delay: index * 0.2 }}
              >
                <FighterCard fighter={fighter} />
              </motion.div>
            ))}
          </div>

          <motion.div
            className="text-center mt-12"
            initial={{ opacity: 0 }}
            whileInView={{ opacity: 1 }}
            viewport={{ once: true }}
          >
            <a
              href="/fighters"
              className="inline-block px-8 py-4 bg-morengy-green hover:bg-morengy-green-dark text-white font-montserrat font-bold rounded-lg transition-all duration-300 transform hover:scale-105"
            >
              View All Fighters
            </a>
          </motion.div>
        </div>
      </section>

      {/* Latest News Section */}
      <section className="py-20 bg-morengy-dark-bg">
        <div className="container mx-auto px-4">
          <SectionHeading
            title="Latest News"
            subtitle="Stay updated on game development, fighters, and cultural events"
          />

          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-8 max-w-7xl mx-auto">
            {latestNews.map((post) => (
              <NewsCard key={post.id} post={post} />
            ))}
          </div>

          <motion.div
            className="text-center mt-12"
            initial={{ opacity: 0 }}
            whileInView={{ opacity: 1 }}
            viewport={{ once: true }}
          >
            <a
              href="/news"
              className="inline-block px-8 py-4 bg-morengy-red hover:bg-morengy-red-dark text-white font-montserrat font-bold rounded-lg transition-all duration-300 transform hover:scale-105"
            >
              Read All News
            </a>
          </motion.div>
        </div>
      </section>

      {/* Call to Action Section */}
      <section className="py-20 bg-gradient-to-br from-morengy-red-dark via-morengy-black to-morengy-green-dark relative overflow-hidden">
        <div className="absolute inset-0 bg-[radial-gradient(circle_at_center,_var(--tw-gradient-stops))] from-morengy-red/20 via-transparent to-morengy-green/20" />
        <div className="container mx-auto px-4 relative z-10">
          <motion.div
            className="max-w-3xl mx-auto text-center"
            initial={{ opacity: 0, y: 20 }}
            whileInView={{ opacity: 1, y: 0 }}
            viewport={{ once: true }}
          >
            <h2 className="text-3xl md:text-4xl lg:text-5xl font-montserrat font-black text-morengy-white mb-6">
              Join the MORENGY Movement
            </h2>
            <p className="text-lg text-morengy-white/80 mb-8">
              Be part of preserving and celebrating Malagasy culture. Whether
              you&apos;re a gamer, cultural enthusiast, or supporter of authentic
              representation in media, there&apos;s a place for you in our community.
            </p>
            <div className="flex flex-col sm:flex-row gap-4 justify-center">
              <a
                href="/contact"
                className="px-8 py-4 bg-morengy-red hover:bg-morengy-red-dark text-white font-montserrat font-bold rounded-lg transition-all duration-300 transform hover:scale-105"
              >
                Get Involved
              </a>
              <a
                href="/about"
                className="px-8 py-4 bg-morengy-white/10 hover:bg-morengy-white/20 border-2 border-morengy-white text-white font-montserrat font-bold rounded-lg transition-all duration-300 transform hover:scale-105"
              >
                Learn More
              </a>
            </div>
          </motion.div>
        </div>
      </section>
    </div>
  );
}

interface FeatureCardProps {
  icon: React.ReactNode;
  title: string;
  description: string;
}

function FeatureCard({ icon, title, description }: FeatureCardProps) {
  return (
    <motion.div
      className="bg-morengy-black border border-morengy-red/30 rounded-lg p-6 text-center hover:border-morengy-green/50 transition-all duration-300"
      whileHover={{ y: -5 }}
      initial={{ opacity: 0, y: 20 }}
      whileInView={{ opacity: 1, y: 0 }}
      viewport={{ once: true }}
    >
      <div className="text-morengy-red mb-4 flex justify-center">{icon}</div>
      <h3 className="text-xl font-montserrat font-bold text-morengy-white mb-2">
        {title}
      </h3>
      <p className="text-morengy-white/70 text-sm">{description}</p>
    </motion.div>
  );
}
