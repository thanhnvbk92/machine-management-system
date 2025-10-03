/** @type {import('next').NextConfig} */
const nextConfig = {
  reactStrictMode: true,
  swcMinify: true,
  webpack: (config, { isServer }) => {
    // Fix for paths with spaces
    config.watchOptions = {
      ...config.watchOptions,
      ignored: ['**/node_modules', '**/.next']
    }
    return config
  },
  experimental: {
    esmExternals: false
  }
}

module.exports = nextConfig
