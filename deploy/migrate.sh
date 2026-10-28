#!/bin/sh
# Applies both contexts with the migration identity. Any nonzero exit fails the Job.
set -eu
./catalog-bundle --connection "$MIGRATION_CONNECTION_STRING" --no-color --prefix-output
./audit-bundle --connection "$MIGRATION_CONNECTION_STRING" --no-color --prefix-output
