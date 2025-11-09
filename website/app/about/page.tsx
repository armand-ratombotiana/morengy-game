import type { Metadata } from "next";
import { HeroSection } from "@/components/HeroSection";
import { SectionHeading } from "@/components/SectionHeading";
import { Timeline } from "@/components/Timeline";
import { timelineEvents } from "@/data/timeline";
import { motion } from "framer-motion";

export const metadata: Metadata = {
  title: "About MORENGY",
  description:
    "Discover the rich history and cultural significance of Morengy, the traditional combat sport from northern Madagascar.",
};

export default function AboutPage() {
  return (
    <div className="min-h-screen">
      {/* Hero Section */}
      <HeroSection
        title="The Legacy of MORENGY"
        subtitle="A tradition forged through generations of Malagasy warriors"
      />

      {/* History Section */}
      <section className="py-20 bg-morengy-dark-bg">
        <div className="container mx-auto px-4">
          <SectionHeading
            title="What is Morengy?"
            subtitle="More than combat—a cultural cornerstone of Malagasy life"
          />

          <div className="max-w-4xl mx-auto space-y-6 text-morengy-white/80 leading-relaxed">
            <p>
              Morengy is a traditional form of bare-knuckle fighting that has
              been practiced in northern Madagascar for centuries. Rooted in
              the cultural fabric of Malagasy communities, Morengy is more than
              just physical combat—it is a rite of passage, a test of courage,
              and a way to resolve disputes while maintaining social harmony.
            </p>

            <p>
              Fights typically take place in village squares or designated
              arenas, accompanied by traditional music and drumming. The rhythm
              of the drums sets the pace of the match, creating a unique fusion
              of martial arts and performance art. Spectators gather to witness
              not only the physical prowess of the fighters but also their
              honor, discipline, and respect for tradition.
            </p>

            <p>
              In traditional Morengy, there are few rules: fighters use only
              their fists, and matches continue until one fighter concedes or is
              unable to continue. Despite the intensity, Morengy emphasizes
              mutual respect—fighters often embrace after matches, and victories
              are celebrated by the entire community.
            </p>

            <p>
              Today, Morengy continues to thrive in northern Madagascar, with
              both traditional village matches and organized tournaments that
              attract fighters and spectators from across the region. Modern
              champions carry forward the legacy of legendary fighters while
              adapting the sport for new generations.
            </p>
          </div>
        </div>
      </section>

      {/* Cultural Significance Section */}
      <section className="py-20 bg-morengy-black">
        <div className="container mx-auto px-4">
          <SectionHeading
            title="Cultural Significance"
            subtitle="The social and spiritual dimensions of Morengy"
          />

          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-8 max-w-6xl mx-auto">
            <SignificanceCard
              title="Social Bonding"
              description="Morengy brings communities together, strengthening social ties and providing a space for collective celebration and identity formation."
            />
            <SignificanceCard
              title="Conflict Resolution"
              description="Traditional matches serve as a honorable way to settle disputes, allowing individuals to defend their honor while avoiding prolonged feuds."
            />
            <SignificanceCard
              title="Rite of Passage"
              description="For young men in northern Madagascar, participating in Morengy is often a crucial step in the journey to adulthood and earning community respect."
            />
            <SignificanceCard
              title="Cultural Preservation"
              description="Morengy preserves ancient fighting techniques, ceremonial practices, and traditional music that have been passed down through generations."
            />
            <SignificanceCard
              title="Physical Excellence"
              description="The sport develops strength, agility, and endurance while teaching discipline, strategic thinking, and mental fortitude."
            />
            <SignificanceCard
              title="Spiritual Connection"
              description="Many fighters invoke ancestral spirits before matches, connecting the physical contest with deeper spiritual and cultural beliefs."
            />
          </div>
        </div>
      </section>

      {/* Timeline Section */}
      <section className="py-20 bg-morengy-dark-bg">
        <div className="container mx-auto px-4">
          <SectionHeading
            title="Historical Timeline"
            subtitle="Journey through the evolution of Morengy from ancient tradition to modern sport"
          />

          <div className="max-w-5xl mx-auto mt-16">
            <Timeline events={timelineEvents} />
          </div>
        </div>
      </section>

      {/* Regional Diversity Section */}
      <section className="py-20 bg-morengy-black">
        <div className="container mx-auto px-4">
          <SectionHeading
            title="Regional Fighting Styles"
            subtitle="Each region of northern Madagascar has developed unique approaches to Morengy"
          />

          <div className="grid grid-cols-1 md:grid-cols-2 gap-8 max-w-4xl mx-auto">
            <RegionCard
              region="Antsiranana"
              style="Tsikafara (Swift Strike)"
              description="Known for lightning-fast movements and emphasis on agility over raw power. Fighters from this region are celebrated for their ability to dodge and counter with precision."
            />
            <RegionCard
              region="Sambava"
              style="Vaky Tany (Earth Breaker)"
              description="Characterized by powerful, grounded strikes inspired by the strength of ocean waves. Fighters focus on overwhelming force and resilient defense."
            />
            <RegionCard
              region="Vohemar"
              style="Lohany Mahery (Mighty Head)"
              description="Emphasizes strategic thinking and mental discipline. Matches are often slower-paced but showcase incredible tactical sophistication."
            />
            <RegionCard
              region="Antalaha"
              style="Ala-Nofy (Forest Dream)"
              description="Mystical and unpredictable, inspired by forest animals. Fighters employ unorthodox movements that confuse and disorient opponents."
            />
          </div>
        </div>
      </section>

      {/* Philosophy Section */}
      <section className="py-20 bg-gradient-to-br from-morengy-red-dark via-morengy-black to-morengy-green-dark">
        <div className="container mx-auto px-4">
          <div className="max-w-3xl mx-auto text-center">
            <h2 className="text-3xl md:text-4xl font-montserrat font-black text-morengy-white mb-6">
              The Morengy Philosophy
            </h2>
            <blockquote className="text-xl italic text-morengy-white/90 border-l-4 border-morengy-red pl-6 my-8">
              &quot;True strength is not measured by victory alone, but by the courage
              to enter the arena, the honor shown to your opponent, and the
              respect you earn from your community.&quot;
            </blockquote>
            <p className="text-morengy-white/80 leading-relaxed">
              Morengy teaches that combat is a path to self-discovery and
              community connection. Every match is an opportunity to demonstrate
              bravery, test one&apos;s limits, and honor the ancestral traditions
              that define Malagasy identity. Win or lose, fighters who embody
              these values are celebrated as true champions.
            </p>
          </div>
        </div>
      </section>
    </div>
  );
}

interface SignificanceCardProps {
  title: string;
  description: string;
}

function SignificanceCard({ title, description }: SignificanceCardProps) {
  return (
    <div className="bg-morengy-dark-bg border border-morengy-red/30 rounded-lg p-6 hover:border-morengy-green/50 transition-colors duration-300">
      <h3 className="text-xl font-montserrat font-bold text-morengy-white mb-3">
        {title}
      </h3>
      <p className="text-morengy-white/70 text-sm leading-relaxed">
        {description}
      </p>
    </div>
  );
}

interface RegionCardProps {
  region: string;
  style: string;
  description: string;
}

function RegionCard({ region, style, description }: RegionCardProps) {
  return (
    <div className="bg-morengy-dark-bg border-l-4 border-morengy-green p-6 rounded-r-lg">
      <h3 className="text-2xl font-montserrat font-bold text-morengy-white mb-2">
        {region}
      </h3>
      <p className="text-morengy-red font-semibold mb-3">{style}</p>
      <p className="text-morengy-white/70 text-sm leading-relaxed">
        {description}
      </p>
    </div>
  );
}
