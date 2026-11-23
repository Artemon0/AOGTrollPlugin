const article = document.querySelector('article');
const outline = document.querySelector('#outline');
for (const heading of article.querySelectorAll('h2')) {
  if (!heading.id) heading.id = heading.textContent.toLowerCase().replace(/[^a-z0-9]+/g, '-').replace(/-$/, '');
  const link = document.createElement('a'); link.href = '#' + heading.id; link.textContent = heading.textContent; outline.append(link);
}
for (const pre of article.querySelectorAll('pre')) {
  const button = document.createElement('button'); button.className = 'copy'; button.textContent = 'Copy';
  button.onclick = async () => { try { await navigator.clipboard.writeText(pre.querySelector('code').textContent); button.textContent = 'Copied'; } catch { button.textContent = 'Select text'; } setTimeout(() => button.textContent = 'Copy', 1500); }; pre.append(button);
}
const links = [...document.querySelectorAll('.sidebar a:not(.download)')];
for (const link of links) if (link.pathname.replace(/index\.html$/, '') === location.pathname.replace(/index\.html$/, '')) link.setAttribute('aria-current','page');
const menu = document.querySelector('.mobile-menu');
menu.onclick = () => menu.setAttribute('aria-expanded', document.querySelector('.sidebar').classList.toggle('open'));
const dialog = document.querySelector('#search'), query = document.querySelector('#query'), results = document.querySelector('#results');
let indexPromise;
function loadIndex() { return indexPromise ||= Promise.all(links.map(async link => { try { const response = await fetch(link.href); if (!response.ok) throw Error(); const doc = new DOMParser().parseFromString(await response.text(), 'text/html'); return {title:link.textContent.trim(),url:link.href,text:doc.querySelector('article')?.textContent || ''}; } catch {return {title:link.textContent.trim(),url:link.href,text:''};} })); }
async function refresh() {
  const value = query.value.trim().toLowerCase(), pages = await loadIndex(); if (value !== query.value.trim().toLowerCase()) return;
  results.replaceChildren();
  for (const page of pages.filter(p => `${p.title} ${p.text}`.toLowerCase().includes(value))) { const a = document.createElement('a'); a.href = page.url; a.textContent = page.title; if(value && page.text) {const snippet = document.createElement('small'), clean = page.text.replace(/\s+/g,' '), at = Math.max(0,clean.toLowerCase().indexOf(value)); snippet.textContent = clean.slice(Math.max(0,at-35),at+115); a.append(snippet);} results.append(a); }
  if (!results.children.length) results.textContent = 'No matching documentation.';
}
function openSearch() { if (!dialog.open) dialog.showModal(); query.focus(); refresh(); }
document.querySelector('.search-launch').onclick = openSearch;
document.querySelector('#close-search').onclick = () => dialog.close();
query.oninput = refresh;
document.addEventListener('keydown', e => { if ((e.ctrlKey || e.metaKey) && e.key.toLowerCase() === 'k') { e.preventDefault(); openSearch(); } });
