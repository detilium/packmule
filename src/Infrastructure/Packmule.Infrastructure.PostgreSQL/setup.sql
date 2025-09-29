create table packages (
  id          uuid primary key default gen_random_uuid(),
  name        text unique not null,
  created_at  timestamptz not null default now(),
  modified_at timestamptz not null default now()
);

create table package_versions (
  id           uuid primary key default gen_random_uuid(),
  package_id   uuid not null references packages(id) on delete cascade,
  version      text not null,
  manifest     jsonb not null,                -- package.json for that version (+ dist)
  tarball_url  text not null,
  integrity    text not null,                 -- 'sha512-...'
  deprecated   text null,
  created_at   timestamptz not null default now(),
  unique (package_id, version)
);

create table dist_tags (
  package_id uuid not null references packages(id) on delete cascade,
  tag        text not null,
  version    text not null,
  primary key (package_id, tag)
);

create index on package_versions (package_id);
create index on dist_tags (package_id);
