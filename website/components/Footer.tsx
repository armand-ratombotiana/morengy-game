"use client";

import Link from "next/link";
import Image from "next/image";
import { Github, Linkedin, Youtube, Instagram, Mail } from "lucide-react";

export function Footer() {
  const currentYear = new Date().getFullYear();

  return (
    <footer className="bg-morengy-black border-t border-morengy-red/20 mt-20">
      <div className="container mx-auto px-4 py-12">
        <div className="grid grid-cols-1 md:grid-cols-4 gap-8">
          {/* Brand Section */}
          <div className="space-y-4">
            <div className="flex items-center space-x-3">
              <Image
                src="/icon.svg"
                alt="MORENGY Logo"
                width={36}
                height={36}
              />
              <h3 className="text-2xl font-montserrat font-black">
                <span className="text-morengy-red">MORE</span>
                <span className="text-morengy-green">NGY</span>
              </h3>
            </div>
            <p className="text-morengy-white/70 text-sm">
              Preserving and celebrating the fighting spirit of northern
              Madagascar through interactive entertainment.
            </p>
          </div>

          {/* Quick Links */}
          <div>
            <h4 className="font-montserrat font-bold text-morengy-white mb-4">
              Quick Links
            </h4>
            <ul className="space-y-2">
              <li>
                <Link
                  href="/about"
                  className="text-morengy-white/70 hover:text-morengy-red transition-colors text-sm"
                >
                  About Morengy
                </Link>
              </li>
              <li>
                <Link
                  href="/fighters"
                  className="text-morengy-white/70 hover:text-morengy-red transition-colors text-sm"
                >
                  Fighters
                </Link>
              </li>
              <li>
                <Link
                  href="/game"
                  className="text-morengy-white/70 hover:text-morengy-red transition-colors text-sm"
                >
                  Game Info
                </Link>
              </li>
              <li>
                <Link
                  href="/news"
                  className="text-morengy-white/70 hover:text-morengy-red transition-colors text-sm"
                >
                  News & Events
                </Link>
              </li>
            </ul>
          </div>

          {/* Community */}
          <div>
            <h4 className="font-montserrat font-bold text-morengy-white mb-4">
              Community
            </h4>
            <ul className="space-y-2">
              <li>
                <Link
                  href="/gallery"
                  className="text-morengy-white/70 hover:text-morengy-red transition-colors text-sm"
                >
                  Gallery
                </Link>
              </li>
              <li>
                <Link
                  href="/contact"
                  className="text-morengy-white/70 hover:text-morengy-red transition-colors text-sm"
                >
                  Contact Us
                </Link>
              </li>
              <li>
                <a
                  href="#newsletter"
                  className="text-morengy-white/70 hover:text-morengy-red transition-colors text-sm"
                >
                  Newsletter
                </a>
              </li>
            </ul>
          </div>

          {/* Social Media */}
          <div>
            <h4 className="font-montserrat font-bold text-morengy-white mb-4">
              Follow Us
            </h4>
            <div className="flex space-x-4">
              <a
                href="https://github.com"
                target="_blank"
                rel="noopener noreferrer"
                className="text-morengy-white/70 hover:text-morengy-red transition-colors"
                aria-label="GitHub"
              >
                <Github size={24} />
              </a>
              <a
                href="https://linkedin.com"
                target="_blank"
                rel="noopener noreferrer"
                className="text-morengy-white/70 hover:text-morengy-red transition-colors"
                aria-label="LinkedIn"
              >
                <Linkedin size={24} />
              </a>
              <a
                href="https://youtube.com"
                target="_blank"
                rel="noopener noreferrer"
                className="text-morengy-white/70 hover:text-morengy-red transition-colors"
                aria-label="YouTube"
              >
                <Youtube size={24} />
              </a>
              <a
                href="https://instagram.com"
                target="_blank"
                rel="noopener noreferrer"
                className="text-morengy-white/70 hover:text-morengy-red transition-colors"
                aria-label="Instagram"
              >
                <Instagram size={24} />
              </a>
            </div>
            <div className="mt-4">
              <a
                href="mailto:contact@morengy.com"
                className="text-morengy-white/70 hover:text-morengy-red transition-colors text-sm flex items-center space-x-2"
              >
                <Mail size={16} />
                <span>contact@morengy.com</span>
              </a>
            </div>
          </div>
        </div>

        {/* Bottom Bar */}
        <div className="mt-12 pt-8 border-t border-morengy-white/10 text-center">
          <p className="text-morengy-white/60 text-sm">
            &copy; {currentYear} MORENGY. All rights reserved. Made with respect
            for Malagasy culture and heritage.
          </p>
        </div>
      </div>
    </footer>
  );
}
