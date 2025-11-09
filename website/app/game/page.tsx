import type { Metadata } from "next";
import { HeroSection } from "@/components/HeroSection";
import { SectionHeading } from "@/components/SectionHeading";
import { Gamepad2, Zap, Users, Trophy, Map, Music } from "lucide-react";

export const metadata: Metadata = {
  title: "Game Info",
  description:
    "Learn about MORENGY gameplay mechanics, features, arenas, and what makes this fighting game unique.",
};

export default function GamePage() {
  return (
    <div className="min-h-screen">
      {/* Hero Section */}
      <HeroSection
        title="Experience MORENGY"
        subtitle="A fighting game that honors tradition while delivering modern gameplay"
        primaryCTA={{ text: "Join Beta Waitlist", href: "/contact" }}
      />

      {/* Overview Section */}
      <section className="py-20 bg-morengy-dark-bg">
        <div className="container mx-auto px-4">
          <SectionHeading
            title="Game Overview"
            subtitle="Where cultural authenticity meets competitive fighting"
          />

          <div className="max-w-4xl mx-auto text-center mb-12">
            <p className="text-lg text-morengy-white/80 leading-relaxed">
              MORENGY combines the intensity of traditional fighting games with
              the rich cultural heritage of Madagascar. Every fighter, arena,
              and move is grounded in authentic Morengy traditions, creating an
              experience that educates while it entertains.
            </p>
          </div>

          {/* Feature Grid */}
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-8">
            <FeatureCard
              icon={<Gamepad2 size={32} />}
              title="Dynamic Combat"
              description="Fluid fighting mechanics inspired by real Morengy techniques, with combos, counters, and signature moves unique to each fighter."
            />
            <FeatureCard
              icon={<Users size={32} />}
              title="Diverse Roster"
              description="Play as historical legends and modern champions, each with authentic fighting styles from different regions of northern Madagascar."
            />
            <FeatureCard
              icon={<Map size={32} />}
              title="Authentic Arenas"
              description="Battle in beautifully rendered locations based on real Morengy venues, from village squares to coastal platforms."
            />
            <FeatureCard
              icon={<Music size={32} />}
              title="Traditional Soundtrack"
              description="Immersive audio featuring traditional Malagasy drums, instruments, and songs recorded with local musicians."
            />
            <FeatureCard
              icon={<Trophy size={32} />}
              title="Story Mode"
              description="Experience the journey of a Morengy fighter through tournaments, rivalries, and the path to becoming a champion."
            />
            <FeatureCard
              icon={<Zap size={32} />}
              title="Online Battles"
              description="Challenge players worldwide in ranked matches and tournaments, representing your chosen fighter and region."
            />
          </div>
        </div>
      </section>

      {/* Gameplay Mechanics */}
      <section className="py-20 bg-morengy-black">
        <div className="container mx-auto px-4">
          <SectionHeading
            title="Gameplay Mechanics"
            subtitle="Easy to learn, challenging to master"
          />

          <div className="max-w-5xl mx-auto space-y-8">
            <MechanicCard
              title="Strike System"
              description="Morengy features a three-button strike system: Light, Medium, and Heavy attacks. Chain them together for devastating combos, or use them strategically to break through opponent defenses."
              gradient="from-morengy-red to-morengy-red-dark"
            />
            <MechanicCard
              title="Rhythm Counter"
              description="Unique to MORENGY, the Rhythm Counter mechanic reflects the traditional role of drums in real matches. Time your attacks and blocks to the beat to gain power-ups and special move access."
              gradient="from-morengy-green to-morengy-green-dark"
            />
            <MechanicCard
              title="Honor System"
              description="Your fighting style affects your Honor rating. Respectful combat, clean victories, and traditional techniques increase Honor, unlocking special cosmetics and arena access."
              gradient="from-morengy-white/80 to-morengy-white/60"
            />
            <MechanicCard
              title="Ancestral Spirit"
              description="Build your Spirit meter through combat. When full, unleash your fighter's ultimate technique—a devastating move inspired by their regional fighting style and personal history."
              gradient="from-morengy-red via-morengy-black to-morengy-green"
            />
          </div>
        </div>
      </section>

      {/* Arenas Section */}
      <section className="py-20 bg-morengy-dark-bg">
        <div className="container mx-auto px-4">
          <SectionHeading
            title="Battle Arenas"
            subtitle="Fight in authentic Malagasy locations"
          />

          <div className="grid grid-cols-1 md:grid-cols-2 gap-8 max-w-5xl mx-auto">
            <ArenaCard
              name="Antsiranana Village Square"
              location="Antsiranana"
              description="The heart of the community, surrounded by cheering spectators and traditional decorations. Sunset matches here are legendary, with the fading light casting dramatic shadows."
            />
            <ArenaCard
              name="Sambava Coastal Platform"
              location="Sambava"
              description="A wooden platform overlooking the Indian Ocean. Feel the sea breeze and hear the waves crashing as you fight. Watch your footing—the platform has no boundaries."
            />
            <ArenaCard
              name="Vohemar Sacred Ground"
              location="Vohemar"
              description="An ancient ceremonial site where the greatest fighters have tested themselves for generations. The arena is marked by standing stones and ancestral symbols."
            />
            <ArenaCard
              name="Antalaha Forest Clearing"
              location="Antalaha"
              description="Deep in the vanilla forests, this natural arena is dappled with sunlight filtering through the canopy. Wildlife sounds create an immersive atmosphere."
            />
          </div>
        </div>
      </section>

      {/* Game Modes */}
      <section className="py-20 bg-morengy-black">
        <div className="container mx-auto px-4">
          <SectionHeading
            title="Game Modes"
            subtitle="Multiple ways to experience Morengy"
          />

          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 max-w-6xl mx-auto">
            <GameModeCard
              title="Story Mode"
              description="Follow the journey of each fighter from humble beginnings to championship glory."
            />
            <GameModeCard
              title="Versus"
              description="Local multiplayer battles—challenge your friends to see who truly masters Morengy."
            />
            <GameModeCard
              title="Online Ranked"
              description="Climb the global leaderboard and earn your place among the world's best fighters."
            />
            <GameModeCard
              title="Training"
              description="Master combos, practice timing, and learn the intricacies of each fighter's style."
            />
            <GameModeCard
              title="Tournament"
              description="Compete in bracket-style competitions, just like traditional Morengy events."
            />
            <GameModeCard
              title="Cultural Archive"
              description="Unlock and explore historical information, fighter biographies, and Malagasy traditions."
            />
            <GameModeCard
              title="Rhythm Challenge"
              description="Test your timing with drum-based mini-games that enhance your Rhythm Counter skills."
            />
            <GameModeCard
              title="Custom Match"
              description="Set your own rules, handicaps, and arena choices for unique battle experiences."
            />
          </div>
        </div>
      </section>

      {/* Media Section */}
      <section className="py-20 bg-morengy-dark-bg">
        <div className="container mx-auto px-4">
          <SectionHeading
            title="Media & Screenshots"
            subtitle="A glimpse into the world of MORENGY"
          />

          <div className="grid grid-cols-1 md:grid-cols-2 gap-8 max-w-5xl mx-auto">
            {/* Trailer Placeholder */}
            <div className="bg-morengy-black border border-morengy-red/30 rounded-lg overflow-hidden aspect-video flex items-center justify-center">
              <div className="text-center">
                <div className="text-6xl mb-4">🎬</div>
                <p className="text-morengy-white/60">Game Trailer Coming Soon</p>
              </div>
            </div>

            {/* Gameplay Demo Placeholder */}
            <div className="bg-morengy-black border border-morengy-green/30 rounded-lg overflow-hidden aspect-video flex items-center justify-center">
              <div className="text-center">
                <div className="text-6xl mb-4">🎮</div>
                <p className="text-morengy-white/60">
                  Gameplay Demo Coming Soon
                </p>
              </div>
            </div>
          </div>

          {/* Screenshot Grid */}
          <div className="grid grid-cols-2 md:grid-cols-4 gap-4 mt-8 max-w-5xl mx-auto">
            {[1, 2, 3, 4].map((i) => (
              <div
                key={i}
                className="bg-gradient-to-br from-morengy-red/20 to-morengy-green/20 border border-morengy-white/20 rounded-lg aspect-video flex items-center justify-center"
              >
                <span className="text-morengy-white/40">Screenshot {i}</span>
              </div>
            ))}
          </div>
        </div>
      </section>

      {/* Beta Sign-up CTA */}
      <section className="py-20 bg-gradient-to-br from-morengy-red-dark via-morengy-black to-morengy-green-dark">
        <div className="container mx-auto px-4">
          <div className="max-w-3xl mx-auto text-center">
            <h2 className="text-3xl md:text-4xl font-montserrat font-black text-morengy-white mb-6">
              Be Among the First to Play
            </h2>
            <p className="text-lg text-morengy-white/80 mb-8">
              Sign up for early access to our mobile demo and be part of shaping
              the future of MORENGY. Beta testers get exclusive fighter skins
              and insider updates.
            </p>
            <a
              href="/contact"
              className="inline-block px-8 py-4 bg-morengy-green hover:bg-morengy-green-dark text-white font-montserrat font-bold rounded-lg transition-all duration-300 transform hover:scale-105"
            >
              Join Beta Waitlist
            </a>
          </div>
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
    <div className="bg-morengy-black border border-morengy-red/30 rounded-lg p-6 hover:border-morengy-green/50 transition-colors duration-300">
      <div className="text-morengy-red mb-4">{icon}</div>
      <h3 className="text-xl font-montserrat font-bold text-morengy-white mb-3">
        {title}
      </h3>
      <p className="text-morengy-white/70 text-sm leading-relaxed">
        {description}
      </p>
    </div>
  );
}

interface MechanicCardProps {
  title: string;
  description: string;
  gradient: string;
}

function MechanicCard({ title, description, gradient }: MechanicCardProps) {
  return (
    <div className={`bg-gradient-to-r ${gradient} rounded-lg p-6 md:p-8`}>
      <h3 className="text-2xl font-montserrat font-bold text-morengy-dark-bg mb-3">
        {title}
      </h3>
      <p className="text-morengy-dark-bg/90 leading-relaxed">{description}</p>
    </div>
  );
}

interface ArenaCardProps {
  name: string;
  location: string;
  description: string;
}

function ArenaCard({ name, location, description }: ArenaCardProps) {
  return (
    <div className="bg-morengy-black border border-morengy-green/30 rounded-lg overflow-hidden">
      {/* Arena Image Placeholder */}
      <div className="h-48 bg-gradient-to-br from-morengy-green/30 to-morengy-red/30 flex items-center justify-center">
        <div className="text-4xl">🏟️</div>
      </div>

      {/* Arena Info */}
      <div className="p-6">
        <h3 className="text-xl font-montserrat font-bold text-morengy-white mb-2">
          {name}
        </h3>
        <p className="text-morengy-green text-sm font-semibold mb-3">
          📍 {location}
        </p>
        <p className="text-morengy-white/70 text-sm leading-relaxed">
          {description}
        </p>
      </div>
    </div>
  );
}

interface GameModeCardProps {
  title: string;
  description: string;
}

function GameModeCard({ title, description }: GameModeCardProps) {
  return (
    <div className="bg-morengy-dark-bg border border-morengy-red/30 rounded-lg p-4 hover:border-morengy-green/50 transition-colors duration-300">
      <h4 className="font-montserrat font-bold text-morengy-white mb-2">
        {title}
      </h4>
      <p className="text-morengy-white/70 text-xs leading-relaxed">
        {description}
      </p>
    </div>
  );
}
