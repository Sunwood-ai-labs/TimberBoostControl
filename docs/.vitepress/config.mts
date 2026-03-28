import { defineConfig } from 'vitepress'

const repoUrl = 'https://github.com/Sunwood-ai-labs/TimberBoostControl'

const navItemsEn = [
  { text: 'Getting Started', link: '/getting-started' },
  { text: 'Settings', link: '/settings' },
  { text: 'Architecture', link: '/architecture' },
  { text: 'Troubleshooting', link: '/troubleshooting' },
  { text: 'Releases', link: '/releases/' },
  { text: 'Japanese', link: '/ja/' },
  { text: 'GitHub', link: repoUrl }
]

const navItemsJa = [
  { text: '導入', link: '/ja/getting-started' },
  { text: '設定', link: '/ja/settings' },
  { text: '構成', link: '/ja/architecture' },
  { text: 'トラブル', link: '/ja/troubleshooting' },
  { text: 'リリース', link: '/ja/releases/' },
  { text: 'English', link: '/' },
  { text: 'GitHub', link: repoUrl }
]

const sidebarBase = [
  {
    text: 'TimberBoostControl',
    items: [
      { text: 'Home', link: '/' },
      { text: 'Getting Started', link: '/getting-started' },
      { text: 'Settings', link: '/settings' },
      { text: 'Architecture', link: '/architecture' },
      { text: 'Troubleshooting', link: '/troubleshooting' }
    ]
  },
  {
    text: 'Releases',
    items: [
      { text: 'Overview', link: '/releases/' },
      { text: 'v0.1.0 Release Notes', link: '/releases/v0.1.0' },
      { text: 'v0.1.0 Walkthrough', link: '/guide/articles/timberboostcontrol-v0-1-0' }
    ]
  }
]

const sidebarJa = [
  {
    text: 'TimberBoostControl',
    items: [
      { text: 'ホーム', link: '/ja/' },
      { text: '導入ガイド', link: '/ja/getting-started' },
      { text: '設定リファレンス', link: '/ja/settings' },
      { text: 'アーキテクチャ', link: '/ja/architecture' },
      { text: 'トラブルシューティング', link: '/ja/troubleshooting' }
    ]
  },
  {
    text: 'リリース',
    items: [
      { text: '一覧', link: '/ja/releases/' },
      { text: 'v0.1.0 リリースノート', link: '/ja/releases/v0.1.0' },
      { text: 'v0.1.0 導入記事', link: '/ja/guide/articles/timberboostcontrol-v0-1-0' }
    ]
  }
]

export default defineConfig({
  title: 'TimberBoostControl',
  description: 'A Timberborn DLL mod that reloads settings.json and regenerates blueprint tweaks.',
  base: '/TimberBoostControl/',
  cleanUrls: true,
  lastUpdated: true,
  head: [
    ['link', { rel: 'icon', href: '/TimberBoostControl/boost-icon.png' }],
    ['meta', { name: 'theme-color', content: '#17392f' }],
    ['meta', { property: 'og:type', content: 'website' }],
    ['meta', { property: 'og:title', content: 'TimberBoostControl' }],
    ['meta', { property: 'og:description', content: 'Blueprint-driven Timberborn boosts with a reloadable settings.json workflow.' }]
  ],
  themeConfig: {
    logo: '/boost-icon.png',
    search: {
      provider: 'local'
    },
    socialLinks: [
      { icon: 'github', link: repoUrl }
    ]
  },
  locales: {
    root: {
      label: 'English',
      lang: 'en-US',
      description: 'A Timberborn DLL mod that reloads settings.json and regenerates blueprint tweaks.',
      themeConfig: {
        nav: navItemsEn,
        sidebar: {
          '/': sidebarBase
        },
        outline: {
          label: 'On this page'
        },
        docFooter: {
          prev: 'Previous page',
          next: 'Next page'
        },
        lastUpdated: {
          text: 'Updated at'
        },
        footer: {
          message: 'Released under the MIT License.',
          copyright: 'Copyright (c) 2026 Sunwood-ai-labs'
        }
      }
    },
    ja: {
      label: '日本語',
      lang: 'ja-JP',
      link: '/ja/',
      description: 'settings.json を再読み込みして blueprint override を再生成する Timberborn DLL MOD です。',
      themeConfig: {
        nav: navItemsJa,
        sidebar: {
          '/ja/': sidebarJa
        },
        outline: {
          label: 'このページ'
        },
        docFooter: {
          prev: '前のページ',
          next: '次のページ'
        },
        lastUpdated: {
          text: '最終更新'
        },
        footer: {
          message: 'MIT License で公開しています。',
          copyright: 'Copyright (c) 2026 Sunwood-ai-labs'
        }
      }
    }
  }
})
