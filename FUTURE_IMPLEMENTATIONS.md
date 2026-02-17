# Future Implementations - AppCanzoniere

**Version:** 0.17.0-alpha  
**Last Updated:** 2026-02-01

This document outlines planned features, enhancements, and improvements for future versions of AppCanzoniere.

---

## 🎯 High Priority (v1.0 Release Blockers)

### 1. Complete Offline Mode (Phase 15)
**Priority:** 🔴 Critical  
**Estimated Effort:** 2-3 days

**Requirements:**
- [ ] Apply database migration for OfflineSongs table
- [ ] Implement FileService for MAUI (cross-platform file operations)
- [ ] Create OfflineSongsPage.xaml with list UI
- [ ] Add download button to PublicSongsPage
- [ ] Show download progress indicator
- [ ] Display downloaded indicator (✓ icon)
- [ ] Add storage usage display in settings
- [ ] Implement conflict resolution UI

**Benefits:**
- Users can access songs without internet
- Better UX for travelers/offline scenarios
- Reduced backend API load

---

### 2. Fix Integration Tests (Phase 14)
**Priority:** 🔴 Critical  
**Estimated Effort:** 1-2 days

**Requirements:**
- [ ] Debug 12 failing integration tests
- [ ] Fix WebApplicationFactory database setup
- [ ] Ensure test isolation and cleanup
- [ ] Verify all tests pass

**Benefits:**
- Automated backend API validation
- Confidence in code changes
- CI/CD pipeline reliability

---

### 3. Configuration Management (Phase 14)
**Priority:** 🔴 Critical  
**Estimated Effort:** 1 day

**Requirements:**
- [ ] Add appsettings.json to MAUI project
- [ ] Load backend URL from configuration
- [ ] Support environment-specific configs (dev/staging/prod)
- [ ] Document configuration in SETUP_INSTRUCTIONS.md

**Benefits:**
- Easy environment switching
- No recompilation for URL changes
- Better deployment flexibility

---

### 4. Advanced Search UI (Phase 16)
**Priority:** 🟡 Important  
**Estimated Effort:** 2-3 days

**Requirements:**
- [ ] Create AdvancedSearchPage.xaml
- [ ] Create AdvancedSearchViewModel
- [ ] Add filter panels (language, key, tags, dates)
- [ ] Add fuzzy search toggle
- [ ] Add sort options dropdown
- [ ] Wire up to AdvancedSearchAsync repository method

**Benefits:**
- Power users can find songs quickly
- Better UX for large song collections
- Utilize existing infrastructure

---

## 🚀 Medium Priority (v1.1-1.2)

### 5. Token Refresh Mechanism
**Priority:** 🟡 Important  
**Estimated Effort:** 1-2 days

**Requirements:**
- [ ] Implement automatic token refresh when near expiration
- [ ] Add background service to check token expiration
- [ ] Handle refresh failures gracefully
- [ ] Update SecureStorage with new token

**Benefits:**
- Better UX (no unexpected logouts)
- Reduced re-authentication friction

---

### 6. Dark Mode Support
**Priority:** 🟡 Important  
**Estimated Effort:** 2-3 days

**Requirements:**
- [ ] Create dark theme ResourceDictionary
- [ ] Create theme switcher in SettingsPage
- [ ] Support system theme detection
- [ ] Test all pages in dark mode
- [ ] Ensure accessibility contrast ratios

**Benefits:**
- Better UX in low-light conditions
- Accessibility improvement
- Modern app expectation

---

### 7. Batch Operations UI
**Priority:** 🟡 Important  
**Estimated Effort:** 3-4 days

**Requirements:**
- [ ] Add multi-select mode to song lists
- [ ] Create batch action toolbar
- [ ] Implement bulk tag assignment
- [ ] Implement bulk album assignment
- [ ] Implement bulk transpose
- [ ] Implement bulk delete with confirmation

**Benefits:**
- Efficiency for power users
- Better library management
- Reduced repetitive actions

---

### 8. Chord Diagrams & Visualizations
**Priority:** 🟡 Important  
**Estimated Effort:** 3-5 days

**Requirements:**
- [ ] Create chord diagram generator service
- [ ] Render guitar chord diagrams (SVG or image)
- [ ] Add piano chord visualization
- [ ] Implement capo position calculator
- [ ] Show alternative chord suggestions
- [ ] Add chord library reference page

**Benefits:**
- Beginner-friendly
- Visual learning aid
- Reduced need for external chord charts

---

### 9. Real-Time Collaboration
**Priority:** 🟢 Nice-to-Have  
**Estimated Effort:** 5-7 days

**Requirements:**
- [ ] Implement SignalR for real-time updates
- [ ] Show who's editing a song (presence)
- [ ] Real-time sync of changes
- [ ] Conflict resolution with OT or CRDT
- [ ] Notification system for changes

**Benefits:**
- Multiple users can edit simultaneously
- Better for church/band collaboration
- Modern collaborative experience

---

## 🎨 User Experience Enhancements

### 10. Tablet & Desktop Optimizations
**Priority:** 🟡 Important  
**Estimated Effort:** 3-4 days

**Requirements:**
- [ ] Create master-detail layouts for tablets
- [ ] Optimize for landscape orientation
- [ ] Add keyboard shortcuts (Windows/macOS)
- [ ] Improve touch targets for stylus
- [ ] Test on various screen sizes

**Benefits:**
- Better experience on larger screens
- Productivity boost for desktop users

---

### 11. Gesture Controls
**Priority:** 🟢 Nice-to-Have  
**Estimated Effort:** 2-3 days

**Requirements:**
- [ ] Swipe left/right to transpose
- [ ] Swipe to delete song
- [ ] Pinch to zoom chords
- [ ] Long-press for context menu

**Benefits:**
- More intuitive mobile UX
- Faster actions

---

### 12. Custom Themes & Fonts
**Priority:** 🟢 Nice-to-Have  
**Estimated Effort:** 2-3 days

**Requirements:**
- [ ] Add theme color picker
- [ ] Add font size slider
- [ ] Add font family selector
- [ ] Save user preferences
- [ ] Apply across all pages

**Benefits:**
- Personalization
- Accessibility (dyslexia-friendly fonts)

---

## 🤖 AI & Intelligence Features

### 13. AI-Powered Smart Collections
**Priority:** 🟢 Nice-to-Have  
**Estimated Effort:** 5-7 days  
**RICHIESTA DA PROMPT INIZIALE:** ✅ Yes

**Requirements:**
- [ ] Analyze song catalog (tags, keys, styles)
- [ ] Use clustering algorithm to group songs
- [ ] Suggest automatic playlists
- [ ] Allow user approval/modification
- [ ] Learn from user preferences

**Benefits:**
- Discover related songs
- Auto-generate themed playlists
- Intelligent organization

**Note:** Il prompt iniziale richiedeva "agente AI per raccolte automatiche" basato su tags/liturgia/autore. Feature deferred per focus su core functionality ma algoritmo di clustering è fattibile con ML.NET.

---

### 14. Song Recommendation Engine
**Priority:** 🟢 Nice-to-Have  
**Estimated Effort:** 5-7 days

**Requirements:**
- [ ] Track user song views/plays
- [ ] Implement collaborative filtering
- [ ] Suggest similar songs by key/tags/style
- [ ] "If you like X, try Y" feature
- [ ] Privacy-respecting analytics

**Benefits:**
- Song discovery
- Better user engagement
- Personalized experience

---

### 15. AI Lyric Generation
**Priority:** 🟢 Nice-to-Have  
**Estimated Effort:** 3-5 days

**Requirements:**
- [ ] Integrate with GPT API
- [ ] Generate lyrics based on theme/style
- [ ] Suggest rhyme schemes
- [ ] Provide lyric templates
- [ ] Clear AI disclaimer

**Benefits:**
- Songwriting assistance
- Creative inspiration
- Time-saving for composers

---

## 🎵 Music Features

### 16. Audio Playback with Metronome
**Priority:** 🟡 Important  
**Estimated Effort:** 4-6 days

**Requirements:**
- [ ] Implement audio player for MP3
- [ ] Add metronome with BPM control
- [ ] Sync lyrics/chords with playback
- [ ] Auto-scroll during playback
- [ ] Loop sections feature

**Benefits:**
- Practice tool
- Performance aid
- Better learning experience

---

### 17. Recording & Practice Mode
**Priority:** 🟢 Nice-to-Have  
**Estimated Effort:** 5-7 days

**Requirements:**
- [ ] Record audio within app
- [ ] Save recordings with song
- [ ] Playback with original track
- [ ] Practice mode (slow down tempo)
- [ ] Mark tricky sections

**Benefits:**
- Self-improvement tool
- Track progress
- Share covers

---

### 18. MIDI Support
**Priority:** 🟢 Nice-to-Have  
**Estimated Effort:** 7-10 days

**Requirements:**
- [ ] Import MIDI files
- [ ] Convert MIDI to ChordPro
- [ ] Export ChordPro to MIDI
- [ ] MIDI keyboard integration
- [ ] Virtual piano keyboard

**Benefits:**
- Wider format support
- MIDI musician workflow
- Export to DAWs

---

## 📱 Backend & Sync Enhancements

### 19. Enhanced Backend API
**Priority:** 🟡 Important  
**Estimated Effort:** 5-7 days

**Requirements:**
- [ ] Add pagination to song endpoints
- [ ] Implement rate limiting
- [ ] Add API versioning (v1, v2)
- [ ] Improve error responses
- [ ] Add GraphQL support (optional)

**Benefits:**
- Better performance
- Scalability
- Future-proof API

---

### 20. Real-Time Sync
**Priority:** 🟢 Nice-to-Have  
**Estimated Effort:** 7-10 days

**Requirements:**
- [ ] Implement push notifications
- [ ] Auto-sync on changes
- [ ] Background sync service
- [ ] Conflict detection and resolution
- [ ] Sync status indicators

**Benefits:**
- Always up-to-date
- Better multi-device experience
- Reduced manual syncing

---

### 21. Cloud Backup & Restore
**Priority:** 🟡 Important  
**Estimated Effort:** 3-5 days

**Requirements:**
- [ ] Implement automatic cloud backup
- [ ] Support multiple cloud providers (Dropbox, Drive, OneDrive)
- [ ] One-click restore from backup
- [ ] Backup scheduling (daily, weekly)
- [ ] Backup encryption

**Benefits:**
- Data safety
- Easy device migration
- Peace of mind

---

## 🌍 Internationalization & Localization

### 22. Multi-Language Support
**Priority:** 🟡 Important  
**Estimated Effort:** 4-6 days  
**RICHIESTA DA PROMPT INIZIALE:** ✅ Yes

**Requirements:**
- [ ] Setup .resx resource files
- [ ] Translate UI to Italian and English
- [ ] Support language switcher in settings
- [ ] RTL language support (Arabic, Hebrew)
- [ ] Localized date/time formats

**Benefits:**
- Wider audience
- Better UX for non-English users
- Global reach

**Note:** Il prompt iniziale richiedeva supporto multilingua IT/EN. Feature deferred per prioritizzare core functionality ma infrastructure MAUI supporta .resx out-of-the-box.

---

### 23. Song Language Detection
**Priority:** 🟢 Nice-to-Have  
**Estimated Effort:** 2-3 days

**Requirements:**
- [ ] Auto-detect song language from lyrics
- [ ] Use ML model or API
- [ ] Allow manual override
- [ ] Filter songs by language

**Benefits:**
- Better organization
- Easier multilingual catalog management

---

## 📊 Analytics & Monitoring

### 24. Usage Analytics
**Priority:** 🟡 Important  
**Estimated Effort:** 2-3 days

**Requirements:**
- [ ] Integrate analytics SDK (Firebase, AppCenter)
- [ ] Track app usage patterns
- [ ] Track feature usage
- [ ] Privacy-respecting (anonymized)
- [ ] GDPR compliant

**Benefits:**
- Understand user behavior
- Prioritize features
- Improve UX

---

### 25. Crash Reporting
**Priority:** 🔴 Critical  
**Estimated Effort:** 1-2 days

**Requirements:**
- [ ] Integrate crash reporting (Sentry, AppCenter)
- [ ] Capture unhandled exceptions
- [ ] Send stack traces
- [ ] Add custom context
- [ ] Alert on critical crashes

**Benefits:**
- Rapid bug fixes
- Better stability
- Proactive monitoring

---

### 26. Performance Monitoring
**Priority:** 🟡 Important  
**Estimated Effort:** 2-3 days

**Requirements:**
- [ ] Track app startup time
- [ ] Monitor database query performance
- [ ] Track API response times
- [ ] Memory usage profiling
- [ ] Battery usage optimization

**Benefits:**
- Identify bottlenecks
- Improve responsiveness
- Better user satisfaction

---

## 🔒 Security & Compliance

### 27. Enhanced Security
**Priority:** 🔴 Critical  
**Estimated Effort:** 3-5 days

**Requirements:**
- [ ] Implement API key obfuscation
- [ ] Add certificate pinning
- [ ] Audit SQL injection vulnerabilities
- [ ] Implement rate limiting
- [ ] Add penetration testing

**Benefits:**
- Protect user data
- Prevent abuse
- Production-ready security

---

### 28. GDPR Compliance
**Priority:** 🔴 Critical  
**Estimated Effort:** 2-3 days  
**RICHIESTA DA PROMPT INIZIALE:** ✅ Yes (Copyright & Responsabilità)

**Requirements:**
- [ ] Add Privacy Policy
- [ ] Add Terms of Service
- [ ] Implement data export feature
- [ ] Implement data deletion (right to be forgotten)
- [ ] Cookie consent (if using web analytics)

**Benefits:**
- Legal compliance (EU)
- User trust
- App store requirement

**Note:** Il prompt iniziale richiedeva:
- Schermata Terms & Conditions ✅ (da implementare)
- Disclaimer responsabilità upload ✅ (da implementare)
- UI per dichiarazione diritti canzone ✅ (da implementare)
- Flag stato legale nelle entità ✅ (CopyrightStatus enum già nel domain)
- Privacy policy GDPR compliance ✅ (da implementare)

**Attualmente implementato:**
- CopyrightStatus enum nel Song entity
- IsPublic flag per distinguere canzoni private/pubbliche
- CreatedBy tracking

**Da implementare:**
- UI flow per Terms & Conditions all'upload
- Checkbox "Sono responsabile del contenuto e dei relativi diritti"
- Schermata Privacy Policy
- GDPR compliance completo

---

### 29. Two-Factor Authentication (2FA)
**Priority:** 🟢 Nice-to-Have  
**Estimated Effort:** 4-6 days

**Requirements:**
- [ ] Implement TOTP (Time-based OTP)
- [ ] Support authenticator apps
- [ ] SMS fallback (optional)
- [ ] Backup codes
- [ ] Enforce 2FA for sensitive actions

**Benefits:**
- Enhanced account security
- Protection against account takeover
- Enterprise-ready

---

## 🎓 Community & Sharing

### 30. Public Song Marketplace
**Priority:** 🟢 Nice-to-Have  
**Estimated Effort:** 10-15 days

**Requirements:**
- [ ] Enhanced search in public database
- [ ] User ratings and reviews
- [ ] Comments on songs
- [ ] Report inappropriate content
- [ ] Moderation tools

**Benefits:**
- Community building
- Content discovery
- User engagement

---

### 31. Social Features
**Priority:** 🟢 Nice-to-Have  
**Estimated Effort:** 7-10 days

**Requirements:**
- [ ] User profiles
- [ ] Follow other users
- [ ] Share playlists
- [ ] Like/favorite songs
- [ ] Activity feed

**Benefits:**
- Social engagement
- Content virality
- User retention

---

### 32. Collaborative Playlists
**Priority:** 🟢 Nice-to-Have  
**Estimated Effort:** 5-7 days

**Requirements:**
- [ ] Share albums/playlists with others
- [ ] Collaborative editing permissions
- [ ] Real-time updates
- [ ] Version history
- [ ] Comments and discussions

**Benefits:**
- Team collaboration
- Church music planning
- Band setlist management

---

## 🎼 Advanced Music Theory

### 33. Chord Progression Analysis
**Priority:** 🟢 Nice-to-Have  
**Estimated Effort:** 5-7 days

**Requirements:**
- [ ] Analyze chord progressions
- [ ] Identify common patterns (I-IV-V, etc.)
- [ ] Suggest chord substitutions
- [ ] Highlight key changes
- [ ] Music theory lessons

**Benefits:**
- Educational tool
- Songwriting assistance
- Music theory learning

---

### 34. Transpose to Best Key for Voice
**Priority:** 🟢 Nice-to-Have  
**Estimated Effort:** 3-5 days

**Requirements:**
- [ ] User vocal range input
- [ ] Analyze melody range
- [ ] Suggest optimal key
- [ ] Auto-transpose to fit voice
- [ ] Capo suggestions

**Benefits:**
- Singability
- Vocal comfort
- Performance optimization

---

## 📱 Platform-Specific Enhancements

### 35. Apple Watch Companion
**Priority:** 🟢 Nice-to-Have  
**Estimated Effort:** 7-10 days

**Requirements:**
- [ ] View song list on watch
- [ ] Quick song selection
- [ ] Display current song chords
- [ ] Remote control metronome
- [ ] Haptic feedback for tempo

**Benefits:**
- Hands-free performance aid
- Quick song reference during church service

---

### 36. Android Wear Companion
**Priority:** 🟢 Nice-to-Have  
**Estimated Effort:** 7-10 days

**Requirements:**
- [ ] Similar to Apple Watch features
- [ ] Offline song access
- [ ] Voice control
- [ ] Lyrics display

**Benefits:**
- Android ecosystem support
- Parity with iOS experience

---

### 37. Browser Extension (Chord Scraper)
**Priority:** 🟢 Nice-to-Have  
**Estimated Effort:** 5-7 days  
**RICHIESTA DA PROMPT INIZIALE:** ⚠️ Partial (Scraping con limitazioni legali)

**Requirements:**
- [ ] Chrome/Firefox extension
- [ ] One-click import from chord websites
- [ ] Parse chords automatically
- [ ] Send to app via API
- [ ] Privacy-respecting (user consent)

**Benefits:**
- Easy import from web
- Time-saving
- Larger song library quickly

**⚠️ NOTE LEGALI:** 
Il prompt iniziale menzionava "scraping privato" con limitazioni:
- "L'uso deve rispettare i Termini d'Uso e il copyright dei siti"
- "L'app deve essere progettata tenendo conto delle limitazioni legali"
- "Non incoraggiare scraping illegale"

**Implementazione raccomandata:**
- Extension che richiede esplicito consenso utente
- Rispetto robots.txt dei siti
- User agent identificabile
- Rate limiting per evitare abuse
- Clear disclaimer su responsabilità utente

**Alternative legali:**
- Partnerships con siti di accordi esistenti (licensing)
- API ufficiali dove disponibili
- Import manuale tramite copy-paste
- Focus su canzoni di dominio pubblico o con licenze aperte

---

## 🎯 Performance & Optimization

### 38. Database Optimization
**Priority:** 🟡 Important  
**Estimated Effort:** 2-3 days

**Requirements:**
- [ ] Add database indexes for common queries
- [ ] Optimize N+1 query problems
- [ ] Implement lazy loading
- [ ] Database vacuum/cleanup
- [ ] Query performance profiling

**Benefits:**
- Faster app
- Better scalability
- Smoother UX

---

### 39. Image & Media Compression
**Priority:** 🟡 Important  
**Estimated Effort:** 2-3 days

**Requirements:**
- [ ] Compress images on upload
- [ ] Generate thumbnails
- [ ] Lazy load images
- [ ] Cache media files
- [ ] Progressive image loading

**Benefits:**
- Reduced storage usage
- Faster loading
- Lower bandwidth consumption

---

### 40. Startup Time Optimization
**Priority:** 🟡 Important  
**Estimated Effort:** 2-3 days

**Requirements:**
- [ ] Profile app startup
- [ ] Defer non-critical initialization
- [ ] Lazy load services
- [ ] Optimize splash screen
- [ ] Target <2s startup time

**Benefits:**
- Better first impression
- Improved UX
- Competitive advantage

---

## 📚 Documentation & Developer Experience

### 41. Comprehensive User Manual
**Priority:** 🟡 Important  
**Estimated Effort:** 3-5 days

**Requirements:**
- [ ] Create user guide (IT/EN)
- [ ] Add screenshots and tutorials
- [ ] Video walkthroughs
- [ ] FAQ section
- [ ] Troubleshooting guide

**Benefits:**
- Reduced support requests
- Better user onboarding
- Self-service help

---

### 42. Developer Documentation
**Priority:** 🟡 Important  
**Estimated Effort:** 2-3 days

**Requirements:**
- [ ] Architecture diagrams
- [ ] API documentation (complete Swagger)
- [ ] Code contribution guidelines
- [ ] Setup guide for contributors
- [ ] Code style guide

**Benefits:**
- Open-source contributions
- Easier onboarding for developers
- Better maintainability

---

## 🚀 Deployment & Distribution

### 43. Google Play Store Release
**Priority:** 🔴 Critical  
**Estimated Effort:** 3-5 days

**Requirements:**
- [ ] Create signed APK/AAB
- [ ] Create store listing
- [ ] Prepare screenshots
- [ ] Write store description
- [ ] Submit for review

**Benefits:**
- Android user acquisition
- Credibility
- Easy distribution

---

### 44. Apple App Store Release
**Priority:** 🔴 Critical  
**Estimated Effort:** 3-5 days

**Requirements:**
- [ ] Create IPA build
- [ ] TestFlight beta testing
- [ ] Create store listing
- [ ] Prepare screenshots
- [ ] Submit for review

**Benefits:**
- iOS user acquisition
- Complete platform coverage

---

### 45. Microsoft Store Release
**Priority:** 🟡 Important  
**Estimated Effort:** 2-3 days

**Requirements:**
- [ ] Create MSIX package
- [ ] Create store listing
- [ ] Windows-specific testing
- [ ] Submit for review

**Benefits:**
- Windows user acquisition
- Desktop market reach

---

## 📊 Business & Monetization (Optional)

### 46. Premium Features / Pro Version
**Priority:** 🟢 Future  
**Estimated Effort:** Variable

**Potential Premium Features:**
- [ ] Unlimited cloud storage
- [ ] Advanced AI features
- [ ] Priority support
- [ ] Remove ads (if any)
- [ ] Exclusive chord diagrams library

**Benefits:**
- Revenue generation
- Sustainable development
- Advanced user segment

---

### 47. Church/Organization Licenses
**Priority:** 🟢 Future  
**Estimated Effort:** Variable

**Requirements:**
- [ ] Multi-user license management
- [ ] Team features
- [ ] Admin console
- [ ] Custom branding
- [ ] Support SLA

**Benefits:**
- Enterprise revenue
- Larger user base
- Professional credibility

---

## 📝 Notes

### Implementation Priority Legend
- 🔴 **Critical:** Required for v1.0 release or addresses major bug / **RICHIESTA DA PROMPT INIZIALE**
- 🟡 **Important:** Significant UX/feature improvement
- 🟢 **Nice-to-Have:** Enhancement or future innovation

### Effort Estimates
All estimates are approximate and may vary based on complexity and testing requirements.

### Features dal Prompt Iniziale
Le seguenti features erano richieste nel prompt_iniziale.md:
- ✅ **Implementate (10/13):** Canzoni, transpose, DB privato/pubblico, metadati ricchi, AI chords, OCR, audio recognition, album manuali, cloud sync prep, backend API
- 🔄 **Parziali (1/13):** Offline mode (infrastructure 70%, UI pending)
- ⚠️ **Da implementare (2/13):** 
  - #13: AI Smart Collections (raccolte automatiche)
  - #22: Multilanguage IT/EN
  - #28: Copyright & responsabilità (Terms & Conditions UI)
  - #37: Scraping privato (con limitazioni legali)

### Community Input
We welcome community suggestions! File a GitHub issue with the "feature-request" label to propose new features.

---

## 🗺️ Roadmap Summary

**v0.17 (Current):** Alpha release with core features  
**v1.0 (Target Q2 2026):** Complete offline mode, fix tests, configuration, advanced search UI  
**v1.1 (Target Q3 2026):** Dark mode, batch operations, token refresh  
**v1.2 (Target Q4 2026):** Chord diagrams, real-time sync, tablet optimizations  
**v2.0 (Future):** AI collections, collaboration, marketplace  

---

_Last updated: 2026-02-01_
