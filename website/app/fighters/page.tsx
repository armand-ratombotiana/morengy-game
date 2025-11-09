"use client";

import { useState } from "react";
import type { Metadata } from "next";
import { motion } from "framer-motion";
import { HeroSection } from "@/components/HeroSection";
import { SectionHeading } from "@/components/SectionHeading";
import { FighterCard } from "@/components/FighterCard";
import { fighters } from "@/data/fighters";
import { Filter } from "lucide-react";

export default function FightersPage() {
  const [selectedEra, setSelectedEra] = useState<"all" | "traditional" | "modern">("all");
  const [selectedRegion, setSelectedRegion] = useState<string>("all");

  // Get unique regions
  const regions = Array.from(new Set(fighters.map((f) => f.region)));

  // Filter fighters
  const filteredFighters = fighters.filter((fighter) => {
    const eraMatch = selectedEra === "all" || fighter.era === selectedEra;
    const regionMatch = selectedRegion === "all" || fighter.region === selectedRegion;
    return eraMatch && regionMatch;
  });

  return (
    <div className="min-h-screen">
      {/* Hero Section */}
      <HeroSection
        title="Warriors of MORENGY"
        subtitle="Meet the legendary fighters who embody the spirit of northern Madagascar"
      />

      {/* Fighters Section */}
      <section className="py-20 bg-morengy-dark-bg">
        <div className="container mx-auto px-4">
          <SectionHeading
            title="Fighter Roster"
            subtitle="From traditional masters to modern champions"
          />

          {/* Filter Controls */}
          <motion.div
            className="max-w-4xl mx-auto mb-12"
            initial={{ opacity: 0, y: 20 }}
            animate={{ opacity: 1, y: 0 }}
          >
            <div className="bg-morengy-black border border-morengy-red/30 rounded-lg p-6">
              <div className="flex items-center gap-2 mb-4">
                <Filter size={20} className="text-morengy-red" />
                <h3 className="text-lg font-montserrat font-bold text-morengy-white">
                  Filter Fighters
                </h3>
              </div>

              <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                {/* Era Filter */}
                <div>
                  <label className="block text-sm text-morengy-white/70 mb-2">
                    Era
                  </label>
                  <div className="flex gap-2">
                    <FilterButton
                      active={selectedEra === "all"}
                      onClick={() => setSelectedEra("all")}
                    >
                      All
                    </FilterButton>
                    <FilterButton
                      active={selectedEra === "traditional"}
                      onClick={() => setSelectedEra("traditional")}
                    >
                      Traditional
                    </FilterButton>
                    <FilterButton
                      active={selectedEra === "modern"}
                      onClick={() => setSelectedEra("modern")}
                    >
                      Modern
                    </FilterButton>
                  </div>
                </div>

                {/* Region Filter */}
                <div>
                  <label className="block text-sm text-morengy-white/70 mb-2">
                    Region
                  </label>
                  <select
                    value={selectedRegion}
                    onChange={(e) => setSelectedRegion(e.target.value)}
                    className="w-full bg-morengy-dark-bg border border-morengy-white/20 rounded-lg px-4 py-2 text-morengy-white focus:outline-none focus:border-morengy-green transition-colors"
                  >
                    <option value="all">All Regions</option>
                    {regions.map((region) => (
                      <option key={region} value={region}>
                        {region}
                      </option>
                    ))}
                  </select>
                </div>
              </div>

              {/* Results Count */}
              <div className="mt-4 text-sm text-morengy-white/60">
                Showing {filteredFighters.length} fighter
                {filteredFighters.length !== 1 ? "s" : ""}
              </div>
            </div>
          </motion.div>

          {/* Fighter Grid */}
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-8 max-w-7xl mx-auto">
            {filteredFighters.map((fighter, index) => (
              <motion.div
                key={fighter.id}
                initial={{ opacity: 0, y: 20 }}
                animate={{ opacity: 1, y: 0 }}
                transition={{ delay: index * 0.1 }}
              >
                <FighterCard fighter={fighter} />
              </motion.div>
            ))}
          </div>

          {/* No Results Message */}
          {filteredFighters.length === 0 && (
            <motion.div
              className="text-center py-12"
              initial={{ opacity: 0 }}
              animate={{ opacity: 1 }}
            >
              <p className="text-morengy-white/60 text-lg">
                No fighters found matching your filters.
              </p>
            </motion.div>
          )}
        </div>
      </section>

      {/* Fighter Stats Section */}
      <section className="py-20 bg-morengy-black">
        <div className="container mx-auto px-4">
          <SectionHeading
            title="Understanding Fighter Stats"
            subtitle="What each attribute means in combat"
          />

          <div className="grid grid-cols-1 md:grid-cols-3 gap-8 max-w-4xl mx-auto">
            <StatExplanation
              title="Strength"
              description="Raw power behind each strike. Higher strength means more damage and the ability to break through defenses."
              icon="💪"
            />
            <StatExplanation
              title="Agility"
              description="Speed and reflexes in combat. Agile fighters can dodge attacks and strike quickly with combo chains."
              icon="⚡"
            />
            <StatExplanation
              title="Skill"
              description="Technical mastery and strategic thinking. Skilled fighters excel at timing, precision, and reading opponents."
              icon="🎯"
            />
          </div>
        </div>
      </section>

      {/* Call to Action */}
      <section className="py-20 bg-gradient-to-br from-morengy-red-dark via-morengy-black to-morengy-green-dark">
        <div className="container mx-auto px-4">
          <div className="max-w-3xl mx-auto text-center">
            <h2 className="text-3xl md:text-4xl font-montserrat font-black text-morengy-white mb-6">
              More Fighters Coming Soon
            </h2>
            <p className="text-lg text-morengy-white/80 mb-8">
              We&apos;re continuously researching and adding more legendary fighters
              from northern Madagascar. Stay tuned for updates!
            </p>
            <a
              href="/news"
              className="inline-block px-8 py-4 bg-morengy-green hover:bg-morengy-green-dark text-white font-montserrat font-bold rounded-lg transition-all duration-300 transform hover:scale-105"
            >
              Follow Development
            </a>
          </div>
        </div>
      </section>
    </div>
  );
}

interface FilterButtonProps {
  active: boolean;
  onClick: () => void;
  children: React.ReactNode;
}

function FilterButton({ active, onClick, children }: FilterButtonProps) {
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

interface StatExplanationProps {
  title: string;
  description: string;
  icon: string;
}

function StatExplanation({ title, description, icon }: StatExplanationProps) {
  return (
    <div className="text-center">
      <div className="text-5xl mb-4">{icon}</div>
      <h3 className="text-xl font-montserrat font-bold text-morengy-white mb-3">
        {title}
      </h3>
      <p className="text-morengy-white/70 text-sm leading-relaxed">
        {description}
      </p>
    </div>
  );
}
