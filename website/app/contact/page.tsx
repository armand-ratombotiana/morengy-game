"use client";

import { useState } from "react";
import type { Metadata } from "next";
import { motion } from "framer-motion";
import { HeroSection } from "@/components/HeroSection";
import { SectionHeading } from "@/components/SectionHeading";
import {
  Mail,
  Github,
  Linkedin,
  Youtube,
  Instagram,
  Send,
  MessageSquare,
  Heart,
} from "lucide-react";

export default function ContactPage() {
  const [formData, setFormData] = useState({
    name: "",
    email: "",
    message: "",
  });
  const [status, setStatus] = useState<"idle" | "submitting" | "success">(
    "idle"
  );

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setStatus("submitting");

    // Simulate form submission
    await new Promise((resolve) => setTimeout(resolve, 1500));

    setStatus("success");
    setFormData({ name: "", email: "", message: "" });

    // Reset success message after 5 seconds
    setTimeout(() => setStatus("idle"), 5000);
  };

  const handleChange = (
    e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>
  ) => {
    setFormData({
      ...formData,
      [e.target.name]: e.target.value,
    });
  };

  return (
    <div className="min-h-screen">
      {/* Hero Section */}
      <HeroSection
        title="Get in Touch"
        subtitle="Join our community and be part of preserving Malagasy culture"
      />

      {/* Contact Form Section */}
      <section className="py-20 bg-morengy-dark-bg">
        <div className="container mx-auto px-4">
          <SectionHeading
            title="Contact Us"
            subtitle="We'd love to hear from you"
          />

          <div className="max-w-5xl mx-auto grid grid-cols-1 lg:grid-cols-2 gap-12">
            {/* Contact Form */}
            <motion.div
              initial={{ opacity: 0, x: -20 }}
              animate={{ opacity: 1, x: 0 }}
              transition={{ duration: 0.6 }}
            >
              <div className="bg-morengy-black border border-morengy-red/30 rounded-lg p-8">
                <h3 className="text-2xl font-montserrat font-bold text-morengy-white mb-6">
                  Send us a Message
                </h3>

                <form onSubmit={handleSubmit} className="space-y-6">
                  {/* Name Field */}
                  <div>
                    <label
                      htmlFor="name"
                      className="block text-sm text-morengy-white/80 mb-2"
                    >
                      Your Name
                    </label>
                    <input
                      type="text"
                      id="name"
                      name="name"
                      value={formData.name}
                      onChange={handleChange}
                      required
                      className="w-full bg-morengy-dark-bg border border-morengy-white/20 rounded-lg px-4 py-3 text-morengy-white placeholder-morengy-white/40 focus:outline-none focus:border-morengy-green transition-colors"
                      placeholder="Enter your name"
                    />
                  </div>

                  {/* Email Field */}
                  <div>
                    <label
                      htmlFor="email"
                      className="block text-sm text-morengy-white/80 mb-2"
                    >
                      Email Address
                    </label>
                    <input
                      type="email"
                      id="email"
                      name="email"
                      value={formData.email}
                      onChange={handleChange}
                      required
                      className="w-full bg-morengy-dark-bg border border-morengy-white/20 rounded-lg px-4 py-3 text-morengy-white placeholder-morengy-white/40 focus:outline-none focus:border-morengy-green transition-colors"
                      placeholder="your.email@example.com"
                    />
                  </div>

                  {/* Message Field */}
                  <div>
                    <label
                      htmlFor="message"
                      className="block text-sm text-morengy-white/80 mb-2"
                    >
                      Message
                    </label>
                    <textarea
                      id="message"
                      name="message"
                      value={formData.message}
                      onChange={handleChange}
                      required
                      rows={6}
                      className="w-full bg-morengy-dark-bg border border-morengy-white/20 rounded-lg px-4 py-3 text-morengy-white placeholder-morengy-white/40 focus:outline-none focus:border-morengy-green transition-colors resize-none"
                      placeholder="Tell us what's on your mind..."
                    />
                  </div>

                  {/* Submit Button */}
                  <button
                    type="submit"
                    disabled={status === "submitting"}
                    className="w-full bg-morengy-red hover:bg-morengy-red-dark disabled:bg-morengy-white/20 text-white font-montserrat font-bold py-3 rounded-lg transition-all duration-300 transform hover:scale-105 disabled:hover:scale-100 flex items-center justify-center gap-2"
                  >
                    {status === "submitting" ? (
                      <>
                        <div className="w-5 h-5 border-2 border-white/30 border-t-white rounded-full animate-spin" />
                        Sending...
                      </>
                    ) : (
                      <>
                        <Send size={20} />
                        Send Message
                      </>
                    )}
                  </button>

                  {/* Success Message */}
                  {status === "success" && (
                    <motion.div
                      initial={{ opacity: 0, y: -10 }}
                      animate={{ opacity: 1, y: 0 }}
                      className="bg-morengy-green/20 border border-morengy-green/50 rounded-lg p-4 text-morengy-green text-center"
                    >
                      Message sent successfully! We&apos;ll get back to you soon.
                    </motion.div>
                  )}
                </form>
              </div>
            </motion.div>

            {/* Contact Info */}
            <motion.div
              initial={{ opacity: 0, x: 20 }}
              animate={{ opacity: 1, x: 0 }}
              transition={{ duration: 0.6 }}
              className="space-y-8"
            >
              {/* Email */}
              <ContactInfoCard
                icon={<Mail size={24} />}
                title="Email Us"
                content="contact@morengy.com"
                description="We typically respond within 24-48 hours"
              />

              {/* Social Media */}
              <ContactInfoCard
                icon={<MessageSquare size={24} />}
                title="Follow Us"
                content="Stay connected on social media"
                description="Join our growing community for daily updates"
              />

              <div className="bg-morengy-black border border-morengy-green/30 rounded-lg p-6">
                <h4 className="font-montserrat font-bold text-morengy-white mb-4">
                  Social Channels
                </h4>
                <div className="flex flex-wrap gap-4">
                  <SocialLink
                    href="https://github.com"
                    icon={<Github size={24} />}
                    label="GitHub"
                  />
                  <SocialLink
                    href="https://linkedin.com"
                    icon={<Linkedin size={24} />}
                    label="LinkedIn"
                  />
                  <SocialLink
                    href="https://youtube.com"
                    icon={<Youtube size={24} />}
                    label="YouTube"
                  />
                  <SocialLink
                    href="https://instagram.com"
                    icon={<Instagram size={24} />}
                    label="Instagram"
                  />
                </div>
              </div>

              {/* Support the Project */}
              <ContactInfoCard
                icon={<Heart size={24} />}
                title="Support the Project"
                content="Help us preserve Malagasy culture"
                description="Various ways to contribute: testing, feedback, spreading the word, or sponsorship"
              />
            </motion.div>
          </div>
        </div>
      </section>

      {/* Ways to Get Involved */}
      <section className="py-20 bg-morengy-black">
        <div className="container mx-auto px-4">
          <SectionHeading
            title="Ways to Get Involved"
            subtitle="Join our mission to celebrate Morengy"
          />

          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 max-w-6xl mx-auto">
            <InvolvementCard
              title="Beta Testing"
              description="Be among the first to play and provide valuable feedback on gameplay and cultural accuracy."
            />
            <InvolvementCard
              title="Cultural Expertise"
              description="Share your knowledge of Morengy traditions, fighting techniques, or Malagasy history."
            />
            <InvolvementCard
              title="Community Building"
              description="Help grow our community by sharing the project and organizing local events."
            />
            <InvolvementCard
              title="Sponsorship"
              description="Support development through sponsorship or partnerships that align with our mission."
            />
          </div>
        </div>
      </section>

      {/* FAQ Section */}
      <section className="py-20 bg-morengy-dark-bg">
        <div className="container mx-auto px-4">
          <SectionHeading title="Frequently Asked Questions" />

          <div className="max-w-3xl mx-auto space-y-6">
            <FAQItem
              question="When will the game be released?"
              answer="We're planning a mobile demo in Q1 2025 for beta testers, with early access later in 2025 and full release in 2026. Join our newsletter to stay updated!"
            />
            <FAQItem
              question="What platforms will MORENGY be available on?"
              answer="We're initially focusing on mobile platforms (iOS and Android), with potential expansion to PC and consoles based on community interest and funding."
            />
            <FAQItem
              question="How can I contribute to the project?"
              answer="There are many ways to help! From beta testing to providing cultural insights, spreading the word, or supporting development. Contact us to discuss how you'd like to be involved."
            />
            <FAQItem
              question="Is the game free?"
              answer="Pricing details are still being finalized. We're committed to making the game accessible while sustaining development and supporting Malagasy cultural initiatives."
            />
            <FAQItem
              question="How do you ensure cultural authenticity?"
              answer="We work closely with Malagasy cultural experts, historians, and Morengy practitioners. Every aspect—from fighter moves to arena designs—is researched and validated for authenticity."
            />
          </div>
        </div>
      </section>

      {/* Final CTA */}
      <section className="py-20 bg-gradient-to-br from-morengy-red-dark via-morengy-black to-morengy-green-dark">
        <div className="container mx-auto px-4">
          <div className="max-w-3xl mx-auto text-center">
            <h2 className="text-3xl md:text-4xl font-montserrat font-black text-morengy-white mb-6">
              Let&apos;s Build This Together
            </h2>
            <p className="text-lg text-morengy-white/80 mb-8">
              MORENGY is more than a game—it&apos;s a movement to preserve and
              celebrate Malagasy culture. Whether you&apos;re a gamer, cultural
              enthusiast, or supporter of authentic representation, there&apos;s a
              place for you in our community.
            </p>
          </div>
        </div>
      </section>
    </div>
  );
}

interface ContactInfoCardProps {
  icon: React.ReactNode;
  title: string;
  content: string;
  description: string;
}

function ContactInfoCard({
  icon,
  title,
  content,
  description,
}: ContactInfoCardProps) {
  return (
    <div className="bg-morengy-black border border-morengy-red/30 rounded-lg p-6">
      <div className="flex items-start gap-4">
        <div className="text-morengy-red">{icon}</div>
        <div>
          <h3 className="text-xl font-montserrat font-bold text-morengy-white mb-2">
            {title}
          </h3>
          <p className="text-morengy-green font-semibold mb-2">{content}</p>
          <p className="text-morengy-white/70 text-sm">{description}</p>
        </div>
      </div>
    </div>
  );
}

interface SocialLinkProps {
  href: string;
  icon: React.ReactNode;
  label: string;
}

function SocialLink({ href, icon, label }: SocialLinkProps) {
  return (
    <a
      href={href}
      target="_blank"
      rel="noopener noreferrer"
      className="flex items-center gap-2 px-4 py-2 bg-morengy-dark-bg hover:bg-morengy-white/10 border border-morengy-white/20 rounded-lg text-morengy-white transition-colors"
      aria-label={label}
    >
      {icon}
      <span className="text-sm">{label}</span>
    </a>
  );
}

interface InvolvementCardProps {
  title: string;
  description: string;
}

function InvolvementCard({ title, description }: InvolvementCardProps) {
  return (
    <motion.div
      className="bg-morengy-dark-bg border border-morengy-green/30 rounded-lg p-6 hover:border-morengy-red/50 transition-colors duration-300"
      whileHover={{ y: -5 }}
      initial={{ opacity: 0, y: 20 }}
      whileInView={{ opacity: 1, y: 0 }}
      viewport={{ once: true }}
    >
      <h4 className="font-montserrat font-bold text-morengy-white mb-3">
        {title}
      </h4>
      <p className="text-morengy-white/70 text-sm leading-relaxed">
        {description}
      </p>
    </motion.div>
  );
}

interface FAQItemProps {
  question: string;
  answer: string;
}

function FAQItem({ question, answer }: FAQItemProps) {
  return (
    <motion.div
      className="bg-morengy-black border border-morengy-white/20 rounded-lg p-6"
      initial={{ opacity: 0, y: 20 }}
      whileInView={{ opacity: 1, y: 0 }}
      viewport={{ once: true }}
    >
      <h4 className="text-lg font-montserrat font-bold text-morengy-white mb-3">
        {question}
      </h4>
      <p className="text-morengy-white/70 leading-relaxed">{answer}</p>
    </motion.div>
  );
}
