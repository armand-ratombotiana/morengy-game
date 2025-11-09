"use client";

import { useState } from "react";
import { motion, AnimatePresence } from "framer-motion";
import { Fighter } from "@/types";
import { Sword, Zap, Shield } from "lucide-react";

interface FighterCardProps {
  fighter: Fighter;
}

export function FighterCard({ fighter }: FighterCardProps) {
  const [isFlipped, setIsFlipped] = useState(false);

  return (
    <motion.div
      className="relative h-[500px] cursor-pointer perspective-1000"
      onClick={() => setIsFlipped(!isFlipped)}
      whileHover={{ scale: 1.02 }}
      transition={{ duration: 0.3 }}
    >
      <AnimatePresence mode="wait">
        {!isFlipped ? (
          <motion.div
            key="front"
            initial={{ rotateY: 0 }}
            exit={{ rotateY: 90 }}
            transition={{ duration: 0.3 }}
            className="absolute inset-0 backface-hidden"
          >
            <div className="h-full bg-gradient-to-br from-morengy-black to-morengy-red-dark border-2 border-morengy-red/30 rounded-lg overflow-hidden shadow-2xl">
              {/* Fighter Image Placeholder */}
              <div className="h-64 bg-gradient-to-br from-morengy-red/20 to-morengy-green/20 flex items-center justify-center relative overflow-hidden">
                <div className="absolute inset-0 bg-[radial-gradient(circle_at_center,_var(--tw-gradient-stops))] from-morengy-red/30 via-transparent to-transparent" />
                <div className="text-6xl font-montserrat font-black text-morengy-white/20 z-10">
                  {fighter.name.charAt(0)}
                </div>
              </div>

              {/* Fighter Info */}
              <div className="p-6">
                <h3 className="text-2xl font-montserrat font-bold text-morengy-white mb-2">
                  {fighter.name}
                </h3>
                <p className="text-morengy-green text-sm font-semibold mb-1">
                  {fighter.style}
                </p>
                <p className="text-morengy-white/60 text-sm mb-4">
                  {fighter.region} • {fighter.era === "traditional" ? "Traditional Era" : "Modern Era"}
                </p>

                {/* Stats */}
                <div className="space-y-2">
                  <StatBar
                    icon={<Sword size={16} />}
                    label="Strength"
                    value={fighter.stats.strength}
                    color="red"
                  />
                  <StatBar
                    icon={<Zap size={16} />}
                    label="Agility"
                    value={fighter.stats.agility}
                    color="green"
                  />
                  <StatBar
                    icon={<Shield size={16} />}
                    label="Skill"
                    value={fighter.stats.skill}
                    color="white"
                  />
                </div>

                <p className="text-morengy-white/40 text-xs mt-4 text-center">
                  Click to learn more
                </p>
              </div>
            </div>
          </motion.div>
        ) : (
          <motion.div
            key="back"
            initial={{ rotateY: -90 }}
            animate={{ rotateY: 0 }}
            transition={{ duration: 0.3 }}
            className="absolute inset-0 backface-hidden"
          >
            <div className="h-full bg-gradient-to-br from-morengy-black to-morengy-green-dark border-2 border-morengy-green/30 rounded-lg overflow-hidden shadow-2xl p-6 overflow-y-auto">
              <h3 className="text-xl font-montserrat font-bold text-morengy-white mb-4">
                About {fighter.name}
              </h3>

              <div className="space-y-4 text-sm">
                <div>
                  <h4 className="text-morengy-green font-semibold mb-2">
                    Biography
                  </h4>
                  <p className="text-morengy-white/80">{fighter.biography}</p>
                </div>

                <div>
                  <h4 className="text-morengy-green font-semibold mb-2">
                    Achievements
                  </h4>
                  <ul className="list-disc list-inside text-morengy-white/80 space-y-1">
                    {fighter.achievements.map((achievement, index) => (
                      <li key={index}>{achievement}</li>
                    ))}
                  </ul>
                </div>

                <div>
                  <h4 className="text-morengy-green font-semibold mb-2">
                    Signature Moves
                  </h4>
                  <div className="flex flex-wrap gap-2">
                    {fighter.moves.map((move, index) => (
                      <span
                        key={index}
                        className="px-3 py-1 bg-morengy-green/20 border border-morengy-green/30 rounded-full text-xs text-morengy-white"
                      >
                        {move}
                      </span>
                    ))}
                  </div>
                </div>
              </div>

              <p className="text-morengy-white/40 text-xs mt-4 text-center">
                Click to flip back
              </p>
            </div>
          </motion.div>
        )}
      </AnimatePresence>
    </motion.div>
  );
}

interface StatBarProps {
  icon: React.ReactNode;
  label: string;
  value: number;
  color: "red" | "green" | "white";
}

function StatBar({ icon, label, value, color }: StatBarProps) {
  const colorClasses = {
    red: "bg-morengy-red",
    green: "bg-morengy-green",
    white: "bg-morengy-white",
  };

  return (
    <div className="flex items-center gap-2">
      <div className="text-morengy-white/60">{icon}</div>
      <div className="flex-1">
        <div className="flex justify-between text-xs text-morengy-white/80 mb-1">
          <span>{label}</span>
          <span>{value}</span>
        </div>
        <div className="h-2 bg-morengy-white/10 rounded-full overflow-hidden">
          <motion.div
            className={`h-full ${colorClasses[color]}`}
            initial={{ width: 0 }}
            animate={{ width: `${value}%` }}
            transition={{ duration: 1, ease: "easeOut" }}
          />
        </div>
      </div>
    </div>
  );
}
