# meowmentum

## Development Environment
You need docker and docker-compose installed on your machine. On Windows, git bash is required to run buildtool. You should also create local `.env` file based on `.env.example`, and fill in missing values.

## Default exposed ports
- **8080**: Nginx instance, composes exposed http endpoints from all services including frontend.
- **8090**: Grafana dashboard.
- **5432**: Postgres database.
- **6379**: Redis cache.

## Buildtool
To access buildtool, run `./meowmentum` in the root directory of the repo. First run will take some time to compile the buildtool itself.

Build cache is stored in `build` directory.

### Commands

<details>
<summary><b>develop</b></summary>

Run local development environment. This will build and start all services and platform dependencies.

**Excluding services:** You can pass names of the services you want to exclude from run after the command name. This allows you to run some services manually for development and debugging. When launching with exclusions, buildtool will output environment variables for excluded service. When running manually, pass these environment variables to the service, so it can connect to the rest of the infrastructure.

**Examples:**

Run all services.
```
$ ./meowmentum develop
```

Run all services except frontend.
```
$ ./meowmentum develop service.web
```

Run all services except frontend and dotnet backend.
```
$ ./meowmentum develop service.web service.dotnet
```

By default, buildtool will only expose ports necessary for excluded services to work and ports that were pinned in `services.yml`. If you want to expose all ports, you can pass `--expose-all` flag.
</details>

<details>
<summary><b>build</b></summary>

Build all services for specified environment. Intended for CI/CD. `--env` flag is required. Pass `--export` flag to export all built images to a tarball.

**Example:**
```
$ ./meowmentum build --env=production --export
```
</details>

<details>
<summary><b>list</b></summary>

List all services and location where they were defined.

**Example:**
```
$ ./meowmentum list
```
</details>

<details>
<summary><b>clean</b></summary>

Wipe buildtool cache. Removes everything from `build` directory except for the buildtool itself.

**Example:**
```
$ ./meowmentum clean
```
</details>

## Service manifest
Services are declared in `services.yml` files. Service structure definition can be found [here](buildtool/service/yaml.go).

<details>
<summary><b>More info</b></summary>

### Path resolution
Some properties in service manifest are file/directory paths. These paths are resolved relative to the directory where the service manifest is located. If path starts with `/`, it is resolved relative to the root directory of the repository. E.g. file `services/web/services.yml` contains `path: src`. This will resolve to `{repo_dir}/services/web/src`. `path: /backend` will resolve to `{repo_dir}/backend`.

### Environment variables
Those are variables that are always available for use in service manifest: `ENVIRONMENT`, `GIT_COMMIT`, `SERVICE`. Note that `SERVICE` is unique for each service and is the name of the service with all dots replaced with dashes. E.g. `service.web` will have `SERVICE=service-web`. Additionally, all variables from `.env.build` file are also available.

### Generators
Generators can be used to generate docker build context before running build. They are written in Go and are located in `buildtool/generators` directory. Generators can be used for both main context and additional contexts. To use a generator, write `gen:<name> [args]` instead of context path. E.g. `gen:frontend` will run `frontend` generator. Generators can be chained. E.g. `gen:nginx ./template.conf` will run `nginx` generator and pass `./template.conf` as an argument.

</details>
