"use client";

import { motion } from "framer-motion";
import { TimelineEvent } from "@/types";
import { Calendar, User, Globe } from "lucide-react";

interface TimelineProps {
  events: TimelineEvent[];
}

export function Timeline({ events }: TimelineProps) {
  return (
    <div className="relative">
      {/* Timeline Line */}
      <div className="absolute left-4 md:left-1/2 top-0 bottom-0 w-0.5 bg-gradient-to-b from-morengy-red via-morengy-green to-morengy-red" />

      {/* Events */}
      <div className="space-y-12">
        {events.map((event, index) => (
          <motion.div
            key={event.id}
            initial={{ opacity: 0, y: 20 }}
            whileInView={{ opacity: 1, y: 0 }}
            viewport={{ once: true }}
            transition={{ delay: index * 0.1 }}
            className={`relative flex items-center ${
              index % 2 === 0
                ? "md:flex-row"
                : "md:flex-row-reverse"
            } flex-col md:gap-8`}
          >
            {/* Content */}
            <div className={`flex-1 ${index % 2 === 0 ? "md:text-right" : "md:text-left"} text-left ml-12 md:ml-0`}>
              <motion.div
                className="bg-morengy-black border border-morengy-red/30 rounded-lg p-6 shadow-lg hover:border-morengy-green/50 transition-colors duration-300"
                whileHover={{ scale: 1.02 }}
              >
                <div className="flex items-center gap-2 mb-2 justify-start md:justify-end">
                  <CategoryIcon category={event.category} />
                  <span className="text-xs text-morengy-white/60 uppercase tracking-wider">
                    {event.category}
                  </span>
                </div>
                <h3 className="text-xl font-montserrat font-bold text-morengy-white mb-2">
                  {event.title}
                </h3>
                <p className="text-morengy-white/70 text-sm">
                  {event.description}
                </p>
              </motion.div>
            </div>

            {/* Year Badge */}
            <div className="absolute left-0 md:relative md:left-auto flex-shrink-0 z-10">
              <div className="w-24 h-24 md:w-32 md:h-32 rounded-full bg-gradient-to-br from-morengy-red to-morengy-green flex items-center justify-center border-4 border-morengy-dark-bg shadow-xl">
                <div className="text-center">
                  <Calendar className="w-6 h-6 md:w-8 md:h-8 text-white mx-auto mb-1" />
                  <div className="text-sm md:text-base font-montserrat font-bold text-white">
                    {event.year}
                  </div>
                </div>
              </div>
            </div>

            {/* Spacer for alternating layout */}
            <div className="flex-1 hidden md:block" />
          </motion.div>
        ))}
      </div>
    </div>
  );
}

function CategoryIcon({ category }: { category: TimelineEvent["category"] }) {
  switch (category) {
    case "Fighter":
      return <User size={14} className="text-morengy-red" />;
    case "Event":
      return <Calendar size={14} className="text-morengy-green" />;
    case "Cultural":
      return <Globe size={14} className="text-morengy-white" />;
  }
}
