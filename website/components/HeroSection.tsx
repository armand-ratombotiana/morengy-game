"use client";

import { motion } from "framer-motion";
import Link from "next/link";
import { FloatingParticles } from "./FloatingParticles";

interface HeroSectionProps {
  title: string;
  subtitle: string;
  primaryCTA?: { text: string; href: string };
  secondaryCTA?: { text: string; href: string };
  backgroundImage?: string;
  showVideo?: boolean;
}

export function HeroSection({
  title,
  subtitle,
  primaryCTA,
  secondaryCTA,
  backgroundImage,
  showVideo = false,
}: HeroSectionProps) {
  return (
    <section className="relative h-screen flex items-center justify-center overflow-hidden">
      {/* Background Video or Image */}
      {showVideo ? (
        <div className="absolute inset-0 z-0">
          {/* Placeholder for video - replace with actual video URL */}
          <div className="absolute inset-0 bg-gradient-to-br from-morengy-red-dark via-morengy-black to-morengy-green-dark animate-pulse" />
          <div className="absolute inset-0 hero-overlay" />
          <FloatingParticles />
        </div>
      ) : (
        <div
          className="absolute inset-0 z-0 bg-cover bg-center"
          style={{
            backgroundImage: backgroundImage
              ? `url(${backgroundImage})`
              : "linear-gradient(135deg, #922B21 0%, #1A1A1A 50%, #1E8449 100%)",
          }}
        >
          <div className="absolute inset-0 hero-overlay" />
          <FloatingParticles />
        </div>
      )}

      {/* Content */}
      <div className="relative z-10 container mx-auto px-4 text-center">
        <motion.div
          initial={{ opacity: 0, y: 30 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ duration: 0.8 }}
          className="max-w-4xl mx-auto"
        >
          <motion.h1
            className="text-4xl md:text-6xl lg:text-7xl font-montserrat font-black mb-6 text-morengy-white"
            initial={{ opacity: 0, y: 20 }}
            animate={{ opacity: 1, y: 0 }}
            transition={{ delay: 0.2, duration: 0.8 }}
          >
            {title}
          </motion.h1>

          <motion.p
            className="text-lg md:text-2xl text-morengy-white/90 mb-8 max-w-2xl mx-auto"
            initial={{ opacity: 0, y: 20 }}
            animate={{ opacity: 1, y: 0 }}
            transition={{ delay: 0.4, duration: 0.8 }}
          >
            {subtitle}
          </motion.p>

          {(primaryCTA || secondaryCTA) && (
            <motion.div
              className="flex flex-col sm:flex-row gap-4 justify-center items-center"
              initial={{ opacity: 0, y: 20 }}
              animate={{ opacity: 1, y: 0 }}
              transition={{ delay: 0.6, duration: 0.8 }}
            >
              {primaryCTA && (
                <Link
                  href={primaryCTA.href}
                  className="relative px-8 py-4 bg-morengy-red hover:bg-morengy-red-dark text-white font-montserrat font-bold rounded-lg transition-all duration-300 transform hover:scale-105 shadow-lg btn-glow overflow-hidden group"
                >
                  <span className="relative z-10">{primaryCTA.text}</span>
                  <div className="absolute inset-0 bg-gradient-to-r from-transparent via-white/20 to-transparent translate-x-[-200%] group-hover:translate-x-[200%] transition-transform duration-700" />
                </Link>
              )}
              {secondaryCTA && (
                <Link
                  href={secondaryCTA.href}
                  className="relative px-8 py-4 bg-morengy-green hover:bg-morengy-green-dark text-white font-montserrat font-bold rounded-lg transition-all duration-300 transform hover:scale-105 shadow-lg overflow-hidden group"
                >
                  <span className="relative z-10">{secondaryCTA.text}</span>
                  <div className="absolute inset-0 bg-gradient-to-r from-transparent via-white/20 to-transparent translate-x-[-200%] group-hover:translate-x-[200%] transition-transform duration-700" />
                </Link>
              )}
            </motion.div>
          )}
        </motion.div>

        {/* Scroll Indicator */}
        <motion.div
          className="absolute bottom-8 left-1/2 transform -translate-x-1/2"
          initial={{ opacity: 0 }}
          animate={{ opacity: 1 }}
          transition={{ delay: 1, duration: 0.8 }}
        >
          <motion.div
            animate={{ y: [0, 10, 0] }}
            transition={{ repeat: Infinity, duration: 1.5 }}
            className="w-6 h-10 border-2 border-morengy-white/50 rounded-full flex items-start justify-center p-2"
          >
            <div className="w-1 h-2 bg-morengy-white/50 rounded-full" />
          </motion.div>
        </motion.div>
      </div>
    </section>
  );
}
