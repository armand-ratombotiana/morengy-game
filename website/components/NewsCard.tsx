"use client";

import { motion } from "framer-motion";
import { NewsPost } from "@/types";
import { Calendar, Tag } from "lucide-react";

interface NewsCardProps {
  post: NewsPost;
}

export function NewsCard({ post }: NewsCardProps) {
  const categoryColors = {
    "Game Development": "morengy-red",
    Fighters: "morengy-green",
    "Cultural Heritage": "morengy-white",
  };

  const categoryColor = categoryColors[post.category];

  return (
    <motion.article
      className="bg-morengy-black border border-morengy-red/30 rounded-lg overflow-hidden shadow-lg hover:border-morengy-green/50 transition-all duration-300"
      whileHover={{ y: -5 }}
      initial={{ opacity: 0, y: 20 }}
      whileInView={{ opacity: 1, y: 0 }}
      viewport={{ once: true }}
    >
      {/* Image Placeholder */}
      <div className="h-48 bg-gradient-to-br from-morengy-red/30 to-morengy-green/30 flex items-center justify-center relative overflow-hidden">
        <div className="absolute inset-0 bg-[radial-gradient(circle_at_center,_var(--tw-gradient-stops))] from-morengy-red/40 via-transparent to-morengy-green/40" />
        <div className="text-4xl font-montserrat font-black text-morengy-white/20 z-10">
          {post.title.charAt(0)}
        </div>
      </div>

      {/* Content */}
      <div className="p-6">
        {/* Category and Date */}
        <div className="flex items-center justify-between mb-3">
          <div className="flex items-center gap-2">
            <Tag size={14} className={`text-${categoryColor}`} />
            <span className={`text-xs font-semibold text-${categoryColor}`}>
              {post.category}
            </span>
          </div>
          <div className="flex items-center gap-2 text-morengy-white/60">
            <Calendar size={14} />
            <time className="text-xs">
              {new Date(post.publishDate).toLocaleDateString("en-US", {
                year: "numeric",
                month: "long",
                day: "numeric",
              })}
            </time>
          </div>
        </div>

        {/* Title */}
        <h3 className="text-xl font-montserrat font-bold text-morengy-white mb-3 line-clamp-2">
          {post.title}
        </h3>

        {/* Summary */}
        <p className="text-morengy-white/70 text-sm line-clamp-3 mb-4">
          {post.summary}
        </p>

        {/* Read More Link */}
        <button className="text-morengy-red hover:text-morengy-green transition-colors font-semibold text-sm">
          Read More →
        </button>
      </div>
    </motion.article>
  );
}
