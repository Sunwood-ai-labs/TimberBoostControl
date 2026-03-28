const fs = require('node:fs')
const path = require('node:path')

const root = process.cwd()

function fail(message) {
  console.error(`[project-validate] ERROR: ${message}`)
  process.exit(1)
}

function mustExist(relativePath) {
  if (!fs.existsSync(path.join(root, relativePath))) {
    fail(`Required file not found: ${relativePath}`)
  }
}

function assertDirectory(relativePath) {
  const fullPath = path.join(root, relativePath)
  if (!fs.existsSync(fullPath)) {
    fail(`Required directory not found: ${relativePath}`)
  }
  if (!fs.statSync(fullPath).isDirectory()) {
    fail(`Expected a directory but found file: ${relativePath}`)
  }
}

function assertHasSourceCode() {
  const sourceDir = path.join(root, 'Source')
  const entries = fs.readdirSync(sourceDir, { withFileTypes: true })
  const sourceFiles = entries.filter((entry) => entry.isFile() && entry.name.endsWith('.cs'))
  if (sourceFiles.length === 0) {
    fail('No C# source files found under Source/.')
  }
}

function assertManifestJson() {
  const manifestPath = path.join(root, 'manifest.json')
  const manifest = JSON.parse(fs.readFileSync(manifestPath, 'utf8'))
  const required = ['Name', 'Version', 'Id', 'MinimumGameVersion', 'Description']
  for (const key of required) {
    if (!manifest[key]) {
      fail(`manifest.json missing required key: ${key}`)
    }
  }
}

function assertWorkflowFiles() {
  const workflowDir = path.join(root, '.github', 'workflows')
  const expected = ['ci.yml', 'deploy-docs.yml']
  for (const fileName of expected) {
    mustExist(path.join('.github', 'workflows', fileName))
  }

  const workflowFiles = fs
    .readdirSync(workflowDir)
    .filter((name) => name.endsWith('.yml') || name.endsWith('.yaml'))
  if (workflowFiles.length !== expected.length) {
    fail(`Expected exactly ${expected.length} workflow files, found ${workflowFiles.length}`)
  }
}

function assertDocsSurface() {
  const requiredDocs = [
    'docs/index.md',
    'docs/getting-started.md',
    'docs/settings.md',
    'docs/architecture.md',
    'docs/troubleshooting.md',
    'docs/ja/index.md',
    'docs/ja/getting-started.md',
    'docs/ja/settings.md',
    'docs/ja/architecture.md',
    'docs/ja/troubleshooting.md',
    'docs/.vitepress/config.mts',
    'docs/.vitepress/theme/index.ts',
    'docs/.vitepress/theme/styles.css',
    'docs/public/boost-icon.png',
    'docs/public/brand/boost-emblem.svg'
  ]

  for (const relativePath of requiredDocs) {
    mustExist(relativePath)
  }
}

function main() {
  console.log('[project-validate] checking repository layout')
  mustExist('README.md')
  mustExist('README.ja.md')
  mustExist('LICENSE')
  mustExist('manifest.json')
  mustExist('settings.json')
  mustExist('build.ps1')
  mustExist('package.json')
  mustExist('scripts/validate-project.js')
  mustExist('scripts/validate-repo.ps1')
  mustExist('Assets/UI/boost-icon.png')
  assertDirectory('Source')
  assertDirectory('.github/workflows')
  assertDirectory('docs')
  assertHasSourceCode()
  assertManifestJson()
  assertWorkflowFiles()
  assertDocsSurface()
  console.log('[project-validate] repository structure is valid')
}

main()
