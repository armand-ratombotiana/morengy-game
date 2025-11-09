// Fighter types
export interface Fighter {
  id: string;
  name: string;
  era: "traditional" | "modern";
  region: string;
  style: string;
  biography: string;
  achievements: string[];
  moves: string[];
  stats: {
    strength: number;
    agility: number;
    skill: number;
  };
  imageUrl?: string;
}

// News/Blog post types
export interface NewsPost {
  id: string;
  title: string;
  summary: string;
  content: string;
  category: "Game Development" | "Fighters" | "Cultural Heritage";
  publishDate: string;
  imageUrl?: string;
  videoUrl?: string;
}

// Gallery item types
export interface GalleryItem {
  id: string;
  type: "image" | "video";
  url: string;
  thumbnail?: string;
  title: string;
  description: string;
  category: "Arena" | "Fighter" | "Culture" | "Event";
}

// Timeline event types
export interface TimelineEvent {
  id: string;
  year: string;
  title: string;
  description: string;
  category: "Fighter" | "Event" | "Cultural";
}

// Contact form types
export interface ContactFormData {
  name: string;
  email: string;
  message: string;
}
