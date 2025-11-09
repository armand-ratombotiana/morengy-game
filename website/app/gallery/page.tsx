"use client";

import { useState } from "react";
import type { Metadata } from "next";
import { motion, AnimatePresence } from "framer-motion";
import { HeroSection } from "@/components/HeroSection";
import { SectionHeading } from "@/components/SectionHeading";
import { galleryItems } from "@/data/gallery";
import { Filter, X, ChevronLeft, ChevronRight } from "lucide-react";
import type { GalleryItem } from "@/types";

export default function GalleryPage() {
  const [selectedCategory, setSelectedCategory] = useState<
    "all" | GalleryItem["category"]
  >("all");
  const [lightboxImage, setLightboxImage] = useState<GalleryItem | null>(null);

  const filteredItems =
    selectedCategory === "all"
      ? galleryItems
      : galleryItems.filter((item) => item.category === selectedCategory);

  const currentIndex = lightboxImage
    ? filteredItems.findIndex((item) => item.id === lightboxImage.id)
    : -1;

  const showNext = () => {
    if (currentIndex < filteredItems.length - 1) {
      setLightboxImage(filteredItems[currentIndex + 1]);
    }
  };

  const showPrevious = () => {
    if (currentIndex > 0) {
      setLightboxImage(filteredItems[currentIndex - 1]);
    }
  };

  return (
    <div className="min-h-screen">
      {/* Hero Section */}
      <HeroSection
        title="Visual Gallery"
        subtitle="Explore the arenas, fighters, and cultural richness of Morengy"
      />

      {/* Gallery Section */}
      <section className="py-20 bg-morengy-dark-bg">
        <div className="container mx-auto px-4">
          <SectionHeading
            title="Media Collection"
            subtitle="Images and videos showcasing the world of MORENGY"
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
                  All Media
                </CategoryButton>
                <CategoryButton
                  active={selectedCategory === "Arena"}
                  onClick={() => setSelectedCategory("Arena")}
                >
                  Arenas
                </CategoryButton>
                <CategoryButton
                  active={selectedCategory === "Fighter"}
                  onClick={() => setSelectedCategory("Fighter")}
                >
                  Fighters
                </CategoryButton>
                <CategoryButton
                  active={selectedCategory === "Culture"}
                  onClick={() => setSelectedCategory("Culture")}
                >
                  Culture
                </CategoryButton>
                <CategoryButton
                  active={selectedCategory === "Event"}
                  onClick={() => setSelectedCategory("Event")}
                >
                  Events
                </CategoryButton>
              </div>

              <div className="mt-4 text-sm text-morengy-white/60">
                Showing {filteredItems.length} item
                {filteredItems.length !== 1 ? "s" : ""}
              </div>
            </div>
          </motion.div>

          {/* Gallery Grid */}
          <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-6 max-w-7xl mx-auto">
            {filteredItems.map((item, index) => (
              <GalleryCard
                key={item.id}
                item={item}
                index={index}
                onClick={() => setLightboxImage(item)}
              />
            ))}
          </div>

          {/* No Results Message */}
          {filteredItems.length === 0 && (
            <motion.div
              className="text-center py-12"
              initial={{ opacity: 0 }}
              animate={{ opacity: 1 }}
            >
              <p className="text-morengy-white/60 text-lg">
                No media found in this category.
              </p>
            </motion.div>
          )}
        </div>
      </section>

      {/* Lightbox */}
      <AnimatePresence>
        {lightboxImage && (
          <motion.div
            className="fixed inset-0 z-50 bg-black/95 flex items-center justify-center p-4"
            initial={{ opacity: 0 }}
            animate={{ opacity: 1 }}
            exit={{ opacity: 0 }}
            onClick={() => setLightboxImage(null)}
          >
            {/* Close Button */}
            <button
              className="absolute top-4 right-4 text-white hover:text-morengy-red transition-colors z-10"
              onClick={() => setLightboxImage(null)}
              aria-label="Close"
            >
              <X size={32} />
            </button>

            {/* Previous Button */}
            {currentIndex > 0 && (
              <button
                className="absolute left-4 text-white hover:text-morengy-green transition-colors z-10"
                onClick={(e) => {
                  e.stopPropagation();
                  showPrevious();
                }}
                aria-label="Previous"
              >
                <ChevronLeft size={48} />
              </button>
            )}

            {/* Next Button */}
            {currentIndex < filteredItems.length - 1 && (
              <button
                className="absolute right-4 text-white hover:text-morengy-green transition-colors z-10"
                onClick={(e) => {
                  e.stopPropagation();
                  showNext();
                }}
                aria-label="Next"
              >
                <ChevronRight size={48} />
              </button>
            )}

            {/* Image Content */}
            <motion.div
              className="max-w-6xl w-full"
              initial={{ scale: 0.9 }}
              animate={{ scale: 1 }}
              exit={{ scale: 0.9 }}
              onClick={(e) => e.stopPropagation()}
            >
              {/* Image Placeholder */}
              <div className="bg-gradient-to-br from-morengy-red/30 to-morengy-green/30 rounded-lg aspect-video flex items-center justify-center mb-4">
                <div className="text-center">
                  <div className="text-8xl mb-4">
                    {lightboxImage.category === "Arena" && "🏟️"}
                    {lightboxImage.category === "Fighter" && "🥊"}
                    {lightboxImage.category === "Culture" && "🎭"}
                    {lightboxImage.category === "Event" && "🎉"}
                  </div>
                  <p className="text-morengy-white/60">
                    {lightboxImage.title}
                  </p>
                </div>
              </div>

              {/* Image Info */}
              <div className="text-center">
                <h3 className="text-2xl font-montserrat font-bold text-white mb-2">
                  {lightboxImage.title}
                </h3>
                <p className="text-white/70">{lightboxImage.description}</p>
                <p className="text-sm text-morengy-green mt-2">
                  {lightboxImage.category}
                </p>
                <p className="text-sm text-white/50 mt-4">
                  {currentIndex + 1} / {filteredItems.length}
                </p>
              </div>
            </motion.div>
          </motion.div>
        )}
      </AnimatePresence>

      {/* Call to Action */}
      <section className="py-20 bg-gradient-to-br from-morengy-red-dark via-morengy-black to-morengy-green-dark">
        <div className="container mx-auto px-4">
          <div className="max-w-3xl mx-auto text-center">
            <h2 className="text-3xl md:text-4xl font-montserrat font-black text-morengy-white mb-6">
              Share Your Morengy Moments
            </h2>
            <p className="text-lg text-morengy-white/80 mb-8">
              Have photos or videos from real Morengy matches or cultural
              events? We&apos;d love to feature authentic content from the community.
            </p>
            <a
              href="/contact"
              className="inline-block px-8 py-4 bg-morengy-green hover:bg-morengy-green-dark text-white font-montserrat font-bold rounded-lg transition-all duration-300 transform hover:scale-105"
            >
              Submit Your Content
            </a>
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

interface GalleryCardProps {
  item: GalleryItem;
  index: number;
  onClick: () => void;
}

function GalleryCard({ item, index, onClick }: GalleryCardProps) {
  const categoryIcons = {
    Arena: "🏟️",
    Fighter: "🥊",
    Culture: "🎭",
    Event: "🎉",
  };

  return (
    <motion.div
      className="group cursor-pointer"
      initial={{ opacity: 0, y: 20 }}
      animate={{ opacity: 1, y: 0 }}
      transition={{ delay: index * 0.05 }}
      whileHover={{ y: -5 }}
      onClick={onClick}
    >
      <div className="bg-morengy-black border border-morengy-red/30 rounded-lg overflow-hidden hover:border-morengy-green/50 transition-colors duration-300">
        {/* Image Placeholder */}
        <div className="aspect-square bg-gradient-to-br from-morengy-red/20 to-morengy-green/20 flex items-center justify-center relative overflow-hidden">
          <div className="text-6xl opacity-40 group-hover:scale-110 transition-transform duration-300">
            {categoryIcons[item.category]}
          </div>
          {/* Overlay on hover */}
          <div className="absolute inset-0 bg-black/60 opacity-0 group-hover:opacity-100 transition-opacity duration-300 flex items-center justify-center">
            <span className="text-white font-semibold">View</span>
          </div>
        </div>

        {/* Card Info */}
        <div className="p-4">
          <div className="flex items-center justify-between mb-2">
            <span className="text-xs text-morengy-green font-semibold">
              {item.category}
            </span>
            <span className="text-xs text-morengy-white/40">
              {item.type === "video" ? "📹" : "🖼️"}
            </span>
          </div>
          <h3 className="font-montserrat font-bold text-morengy-white text-sm line-clamp-2 mb-1">
            {item.title}
          </h3>
          <p className="text-xs text-morengy-white/60 line-clamp-2">
            {item.description}
          </p>
        </div>
      </div>
    </motion.div>
  );
}
