"use client";

import { motion } from "framer-motion";

interface SectionHeadingProps {
  title: string;
  subtitle?: string;
  centered?: boolean;
}

export function SectionHeading({
  title,
  subtitle,
  centered = true,
}: SectionHeadingProps) {
  return (
    <motion.div
      className={`mb-12 ${centered ? "text-center" : ""}`}
      initial={{ opacity: 0, y: 20 }}
      whileInView={{ opacity: 1, y: 0 }}
      viewport={{ once: true }}
      transition={{ duration: 0.6 }}
    >
      <h2 className="text-3xl md:text-4xl lg:text-5xl font-montserrat font-black text-morengy-white mb-4">
        {title}
      </h2>
      {subtitle && (
        <p className="text-lg text-morengy-white/70 max-w-3xl mx-auto">
          {subtitle}
        </p>
      )}
      <div className="mt-6 flex items-center justify-center gap-2">
        <div className="h-1 w-12 bg-morengy-red rounded-full" />
        <div className="h-1 w-12 bg-morengy-green rounded-full" />
        <div className="h-1 w-12 bg-morengy-red rounded-full" />
      </div>
    </motion.div>
  );
}
